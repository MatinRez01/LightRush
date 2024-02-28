using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Scripting;
public class ServerModels
{
    [Serializable]
    public class Response
    {
        public string message;
        public int msgCode;
        public JToken data;
        public ResponseError[] errors;
    }

    [Serializable]
    public class ResponseError
    {
        public string field;
        public string type;
        public string message;
        public JToken expected;
        public JToken actual;
    }

    [Serializable]
    public class Record
    {
        public TopScore topScore;

        [Serializable]
        public class TopScore
        {
            public int topScore;
            public string name;
            public int rank;
        }
    }

    [Serializable, Preserve]
    public class Top10
    {
        public User[] topUsers;
    }

    [Serializable]
    public class Median
    {
        public MedainClass medianUserScore;

        [Serializable]
        public class MedainClass
        {
            public User userWithHigherScore;
            public User userScore;
            public User userWithLowerScore;
           
        }
    }
    [Serializable]
    public class User
    {
        public string name;
        public int topScore;
        public int rank;
    }
}