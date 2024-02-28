using UnityEngine;
using BestHTTP;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class ServerConnection
{
    public const string SERVER_ADDRESS = "https://light-rush.termentika.ir/api/v1/";

    public const string GET_TOP10 = "records/top-ten/";
    public const string GET_MEDIAN = "records/median/";
    public const string POST_RECORD = "records";

    public static void GetTop10Records(Action<ServerModels.Top10> callback)
    {
        var request = new HTTPRequest(new Uri(SERVER_ADDRESS + GET_TOP10), HTTPMethods.Get, (req, res) =>
            {
                if (IsOK(req, res))
                {
                    var response = GetResponse(res.DataAsText);
                    var data = GetResponseData(response);

                    var top10 = JsonConvert.DeserializeObject<ServerModels.Top10>(Convert.ToString(data));
                    callback.Invoke(top10);
                }
            });

        request.Send();
    }

    public static void GetMedianRecords(string phoneNumber, Action<ServerModels.Median> callback)
    {
        var request = new HTTPRequest(new Uri(SERVER_ADDRESS + GET_MEDIAN), HTTPMethods.Post, (req, res) =>
        {
            if (IsOK(req, res))
            {
                var response = GetResponse(res.DataAsText);
                var data = GetResponseData(response);

                var median = JsonConvert.DeserializeObject<ServerModels.Median>(Convert.ToString(data));

                callback?.Invoke(median);
            }
        });

        request.AddField("phoneNumber", phoneNumber);
        request.Send();
    }

    public static void SetNewRecord(string name, string phoneNumber, string score, Action callback)
    {
        if (!LoggedIn()) return;
        var request = new HTTPRequest(new Uri(SERVER_ADDRESS + POST_RECORD), HTTPMethods.Post, (req, res) =>
        {
            if (IsOK(req, res))
            {
                var response = GetResponse(res.DataAsText);
                var data = GetResponseData(response);

                var record = JsonConvert.DeserializeObject<ServerModels.Record>(Convert.ToString(data));

                callback?.Invoke();
            };
        });

        request.AddField("name", name);

        request.AddField("phoneNumber", phoneNumber);
        request.AddField("score", score);

        request.Send();
    }
    public static void SetNewRecord(string name, string score)
    {
        if (!LoggedIn()) return;
        var request = new HTTPRequest(new Uri(SERVER_ADDRESS + POST_RECORD), HTTPMethods.Post, (req, res) =>
        {
            if (IsOK(req, res))
            {
                var response = GetResponse(res.DataAsText);
                var data = GetResponseData(response);

                var record = JsonConvert.DeserializeObject<ServerModels.Record>(Convert.ToString(data));

            };
        });
        string phoneNumber = PlayerPrefs.GetString("PlayerPhoneNumber");
        request.AddField("name", name);
        request.AddField("phoneNumber", phoneNumber);
        request.AddField("score", score);

        request.Send();
    }


    public static bool IsConnected()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            return true;

        return false;
    }
    public static bool LoggedIn()
    {
        return PlayerPrefs.HasKey("PlayerPhoneNumber");
    }
    private static bool IsOK(HTTPRequest req, HTTPResponse res)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (res.IsSuccess)
                    return true;

                Debug.LogWarning(string.Format("Request finished successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                    res.StatusCode,
                    res.Message,
                    res.DataAsText));

                var response = GetResponse(res.DataAsText);
                Debug.Log(response.message);

                if (response.errors.Length > 0)
                {
                    for (int i = 0; i < response.errors.Length; i++)
                    {
                        Debug.Log($"{response.errors[i].field}\n{response.errors[i].type}\n{response.errors[i].message}\n{Convert.ToString(response.errors[i].expected)}\n{Convert.ToString(response.errors[i].actual)}");
                    }
                }

                break;
            case HTTPRequestStates.Error:
                Debug.LogError("Request Finished with Error! " +
                    ((req.Exception != null) ?
                    (req.Exception.Message + "\n" + req.Exception.StackTrace) :
                    "No Exception"));
                break;
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("Request Aborted!");
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError("Connection Timed Out");
                break;
            case HTTPRequestStates.TimedOut:
                Debug.LogError("Proccessing the request Timed Out!");
                break;
            default:
                break;
        }

        return false;
    }

    private static ServerModels.Response GetResponse(string json)
    {
        return JsonConvert.DeserializeObject<ServerModels.Response>(json);
    }

    private static JToken GetResponseData(ServerModels.Response response)
    {
        return JToken.Parse(Convert.ToString(response.data));
    }
}