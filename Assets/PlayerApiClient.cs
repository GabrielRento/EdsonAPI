using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Player
{
    public int id;
    public int vida;
    public int quantidadeItens;
    public float posicaoX;
    public float posicaoY;
    public float posicaoZ;
}

public class PlayerApiClient : MonoBehaviour
{
    public string baseUrl = "http://localhost:5014/api/player/GetPlayers";

    void Start()
    {
        StartCoroutine(GetPlayers());
    }

    IEnumerator GetPlayers()
    {
        using var req = UnityWebRequest.Get(baseUrl);
        req.SetRequestHeader("Accept", "application/json");
        yield return req.SendWebRequest();

        Debug.Log($"Chamou: {baseUrl}  responseCode: {req.responseCode}  error: {req.error}");

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Recebido: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("GetPlayers failed: " + req.error);
        }
    }
}
