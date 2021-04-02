Shader "Custom/RenderSurfaceShader"
{
    SubShader{
            Tags {"Queue" = "Geometry" }
            Pass {
                    Stencil {
                        Ref 1
                        Comp equal

                    }
               }
    }
}
