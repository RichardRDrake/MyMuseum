using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

    // In case of hiccups in the network, each download/upload will attempt 5 times in total, before giving up
    private static int s_NetworkTimesTried = 0;

    /// <summary>
    /// Downloads a text file
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public static IEnumerator DownloadText(string URL)
    {
        // Wait until previous request has finished
        while (isDownloading)
            yield return null;

        isDownloading = true;
        DownloadedStreamFile = null;
        DownloadedTextFile = null;

        // Try 5 times before giving up
        Retry:

        using (UnityWebRequest uwr = UnityWebRequest.Get(URL))
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                // Try again 4 times (1 time a second)
                s_NetworkTimesTried++;
                if (s_NetworkTimesTried < 5)
                {
                    yield return new WaitForSeconds(1);
                    goto Retry;
                }

                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                // Reset times tried as it's used whenever we are trying to download something
                s_NetworkTimesTried = 0;

                MemoryStream stream = new MemoryStream(uwr.downloadHandler.data);
                stream.Position = 0;

                DownloadedStreamFile = stream;
                DownloadedTextFile = uwr.downloadHandler.text;
            }

            uwr.Dispose();
        }

        isDownloading = false;
    }

    /// <summary>
    /// Downloads a text file and sends authorization bearer token
    /// </summary>
    /// <param name="URI"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static IEnumerator DownloadText(string URI, string token)
    {
        // Wait until previous request has finished
        while (isDownloading)
            yield return null;

        isDownloading = true;
        DownloadedStreamFile = null;
        DownloadedTextFile = null;

        // Try 5 times before giving up
        Retry:

        var uwr = UnityWebRequest.Get(URI);

        // Set request header using unique token provided by deep-link
        uwr.SetRequestHeader("Authorization", "Bearer " + token);

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            // Try again 4 times (1 time a second)
            s_NetworkTimesTried++;
            if (s_NetworkTimesTried < 5)
            {
                yield return new WaitForSeconds(1);
                goto Retry;
            }

            Debug.Log("Network Error: " + uwr.error);
            isDownloading = false;
            yield break;
        }
        else
        {
            // Reset times tried as it's used whenever we are trying to download something
            s_NetworkTimesTried = 0;

            MemoryStream stream = new MemoryStream(uwr.downloadHandler.data);
            stream.Position = 0;

            DownloadedStreamFile = stream;
            DownloadedTextFile = uwr.downloadHandler.text;
        }

        uwr.Dispose();

        isDownloading = false;
    }

    /// <summary>
    /// Downloads a myMusuem room file from server
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public static IEnumerator DownloadRoom(string URL)
    {
        // Wait until previous request has finished
        while (isDownloading)
            yield return null;

        isDownloading = true;
        DownloadedRoomFile = null;

        // Try 5 times before giving up
        Retry:

        using (UnityWebRequest uwr = UnityWebRequest.Get(URL))
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                // Try again 4 times (1 time a second)
                s_NetworkTimesTried++;
                if (s_NetworkTimesTried < 5)
                {
                    yield return new WaitForSeconds(1);
                    goto Retry;
                }

                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                // Reset times tried as it's used whenever we are trying to download something
                s_NetworkTimesTried = 0;

                BinaryFormatter formatter = new BinaryFormatter();

                SurrogateSelector selector = new SurrogateSelector();

                Vector3SerializationSurrogate v3Surrogate = new Vector3SerializationSurrogate();
                QuaternionSerializationSurrogate qSurrogate = new QuaternionSerializationSurrogate();

                selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
                selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qSurrogate);

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
    /// Downloads a myMusuem room file from server and send authorization
    /// </summary>
    /// <param name="URI"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static IEnumerator DownloadRoom(string URI, string token)
    {
        // Wait until previous request has finished
        while (isDownloading)
            yield return null;

        isDownloading = true;
        DownloadedRoomFile = null;

        // Try 5 times before giving up
        Retry:

        var uwr = UnityWebRequest.Get(URI);

        // Set request header using unique token provided by deep-link
        uwr.SetRequestHeader("Authorization", "Bearer " + token);

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            // Try again 4 times (1 time a second)
            s_NetworkTimesTried++;
            if (s_NetworkTimesTried < 5)
            {
                yield return new WaitForSeconds(1);
                goto Retry;
            }

            Debug.Log("Network Error: " + uwr.error);
            isDownloading = false;
            yield break;
        }
        else
        {
            // Reset times tried as it's used whenever we are trying to download something
            s_NetworkTimesTried = 0;

            Debug.Log("Received: " + uwr.downloadHandler.text);

            BinaryFormatter formatter = new BinaryFormatter();
            SurrogateSelector selector = new SurrogateSelector();

            Vector3SerializationSurrogate v3Surrogate = new Vector3SerializationSurrogate();
            QuaternionSerializationSurrogate qSurrogate = new QuaternionSerializationSurrogate();

            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qSurrogate);
            formatter.SurrogateSelector = selector;

            // Read the *.save file
            MemoryStream stream = new MemoryStream(uwr.downloadHandler.data);
            DownloadedRoomFile = formatter.Deserialize(stream) as RoomData;

            // Clean up
            stream.Close();
        }

        // Clean up
        uwr.Dispose();

        isDownloading = false;
    }

    /// <summary>
    /// Downloads a texture
    /// </summary>
    /// <param name="URL"></param>
    /// <returns></returns>
    public static IEnumerator DownloadTexture(string URL)
    {
        // Wait until previous request has finished
        while (isDownloading)
            yield return null;

        isDownloading = true;
        DownloadedTexture = null;

        // Try 5 times before giving up
        Retry:

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(URL))
        {
            var asyncOp = uwr.SendWebRequest();

            while (asyncOp.isDone == false)
            {
                yield return null;
            }

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                // Try again 4 times (1 time a second)
                s_NetworkTimesTried++;
                if (s_NetworkTimesTried < 5)
                {
                    yield return new WaitForSeconds(1);
                    goto Retry;
                }

                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                // Reset times tried as it's used whenever we are trying to download something
                s_NetworkTimesTried = 0;

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
        // Wait until previous request has finished
        while (isDownloading)
            yield return null;

        isDownloading = true;
        DownloadedAudioClip = null;

        // Try 5 times before giving up
        Retry:

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

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                // Try again 4 times (1 time a second)
                s_NetworkTimesTried++;
                if (s_NetworkTimesTried < 5)
                {
                    yield return new WaitForSeconds(1);
                    goto Retry;
                }

                Debug.Log(uwr.error);
                isDownloading = false;
                yield break;
            }
            else
            {
                // Reset times tried as it's used whenever we are trying to download something
                s_NetworkTimesTried = 0;

                DownloadedAudioClip = DownloadHandlerAudioClip.GetContent(uwr);
            }

            uwr.Dispose();
        }

        isDownloading = false;
    }
}
