using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Downloads various items from the server
/// </summary>
public class DC_Downloader : MonoBehaviour
{
    public static bool isDownloading = false;

    public static string DownloadedTextFile { get; private set; }
    public static RoomData DownloadedRoomFile { get; private set; }
    public static Texture2D DownloadedTexture { get; private set; }
    public static AudioClip DownloadedAudioClip { get; private set; }

    public static Stream DownloadedStreamFile { get; private set; }

    /// <summary>
    /// Downloads a text file
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public static IEnumerator DownloadText(string URL)
    {
        isDownloading = true;

        using (UnityWebRequest uwr = UnityWebRequest.Get(URL))
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                MemoryStream stream = new MemoryStream(uwr.downloadHandler.data);
                stream.Position = 0;

                DownloadedStreamFile = stream;
                DownloadedTextFile = uwr.downloadHandler.text;
            }

            uwr.Dispose();
        }

        isDownloading = false;
    }

    public static IEnumerator DownloadRoom(string URL)
    {
        isDownloading = true;

        using (UnityWebRequest uwr = UnityWebRequest.Get(URL))
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();

                SurrogateSelector selector = new SurrogateSelector();

                Vector3SerializationSurrogate v3Surrogate = new Vector3SerializationSurrogate();
                QuaternionSerializationSurrogate qSurrogate = new QuaternionSerializationSurrogate();
                //AssetSOSerializationSurrogate aSurrogate = new AssetSOSerializationSurrogate();

                selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
                selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qSurrogate);
                //selector.AddSurrogate(typeof(AssetPlacerScriptableObject), new StreamingContext(StreamingContextStates.All), aSurrogate);

                formatter.SurrogateSelector = selector;

                MemoryStream stream = new MemoryStream(uwr.downloadHandler.data);
                DownloadedRoomFile = formatter.Deserialize(stream) as RoomData;
                stream.Close();
            }

            uwr.Dispose();
        }

        isDownloading = false;
    }

    /// <summary>
    /// Downloads a texture
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public static IEnumerator DownloadTexture(string URL)
    {
        isDownloading = true;

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(URL))
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                DownloadedTexture = DownloadHandlerTexture.GetContent(uwr);
            }

            uwr.Dispose();
        }

        isDownloading = false;
    }

    /// <summary>
    /// Downloads an audio clip
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public static IEnumerator DownloadAudio(string URL)
    {
        isDownloading = true;

#if UNITY_EDITOR
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(URL, AudioType.OGGVORBIS))
#else
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(URL, AudioType.MPEG))
#endif
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                DownloadedAudioClip = DownloadHandlerAudioClip.GetContent(uwr);
            }

            uwr.Dispose();
        }

        isDownloading = false;
    }
}
