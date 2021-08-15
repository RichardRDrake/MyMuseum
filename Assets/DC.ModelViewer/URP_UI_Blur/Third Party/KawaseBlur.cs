using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KawaseBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class KawaseBlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material blurMaterial = null;

        [Range(1,15)]
        public int blurPasses = 1;

        [Range(1,4)]
        public int downsample = 1;
        public bool copyToFramebuffer;
        public string targetName = "_blurTexture";
    }

    public KawaseBlurSettings settings = new KawaseBlurSettings();

    class CustomRenderPass : ScriptableRenderPass
    {
        public Material blurMaterial;
        public int passes;
        public int downsample;
        public bool copyToFramebuffer;
        public string targetName;        
        string profilerTag;

        RenderTargetIdentifier tmpRT1;
        RenderTargetIdentifier tmpRT2;

        // RD EDIT: Optimisations
        private enum PropertyID
        {
            TMP_BLUR_RT_1,
            TMP_BLUR_RT_2,
            OFFSET,
            COUNT
        }
        int[] m_PropertyIDs = new int[(int)PropertyID.COUNT];
        
        private RenderTargetIdentifier source { get; set; }

        public void Setup(RenderTargetIdentifier source) {
            this.source = source;
        }

        public CustomRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width / downsample;
            var height = cameraTextureDescriptor.height / downsample;

            m_PropertyIDs[(int)PropertyID.TMP_BLUR_RT_1] = Shader.PropertyToID("tmpBlurRT1");
            m_PropertyIDs[(int)PropertyID.TMP_BLUR_RT_2] = Shader.PropertyToID("tmpBlurRT2");
            cmd.GetTemporaryRT(m_PropertyIDs[(int)PropertyID.TMP_BLUR_RT_1], width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(m_PropertyIDs[(int)PropertyID.TMP_BLUR_RT_2], width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            tmpRT1 = new RenderTargetIdentifier(m_PropertyIDs[(int)PropertyID.TMP_BLUR_RT_1]);
            tmpRT2 = new RenderTargetIdentifier(m_PropertyIDs[(int)PropertyID.TMP_BLUR_RT_2]);
            
            ConfigureTarget(tmpRT1);
            ConfigureTarget(tmpRT2);

            m_PropertyIDs[(int)PropertyID.OFFSET] = Shader.PropertyToID("_offset");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            // first pass
            cmd.SetGlobalFloat(m_PropertyIDs[(int)PropertyID.OFFSET], 0.1f);
            cmd.Blit(source, tmpRT1, blurMaterial);

            // extra blur passes
            for (int I = 1; I < passes - 1; I++) 
            {
                cmd.SetGlobalFloat(m_PropertyIDs[(int)PropertyID.OFFSET], 0.5f + I);
                cmd.Blit(tmpRT1, tmpRT2, blurMaterial);

                // pingpong
                var rttmp = tmpRT1;
                tmpRT1 = tmpRT2;
                tmpRT2 = rttmp;
            }

            // final pass
            cmd.SetGlobalFloat(m_PropertyIDs[(int)PropertyID.OFFSET], 0.5f + passes - 1f);
            if (copyToFramebuffer) 
            {
                cmd.Blit(tmpRT1, source, blurMaterial);
            } 
            else 
            {
                cmd.Blit(tmpRT1, tmpRT2, blurMaterial);
                cmd.SetGlobalTexture(targetName, tmpRT2);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new CustomRenderPass("KawaseBlur");
        scriptablePass.blurMaterial = settings.blurMaterial;
        scriptablePass.passes = settings.blurPasses;
        scriptablePass.downsample = settings.downsample;
        scriptablePass.copyToFramebuffer = settings.copyToFramebuffer;
        scriptablePass.targetName = settings.targetName;

        scriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        scriptablePass.Setup(src);
        renderer.EnqueuePass(scriptablePass);
    }
}


