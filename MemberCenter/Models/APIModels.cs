using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MemberCenter.Models
{
    public class StatusModel
    {
        public string Version { get; set; }

    }


    public class MessageJSON
    {
        [DataMember(Name = "message")]
        public string MessageString { get; set; }

        [DataMember(Name = "error_code", EmitDefaultValue = false)]
        public int? ErrorCode { get; set; }
    }

    public class APIResponseModel
    {
        public string Message { get; set; }
    }


    public class ConsumeRequestModel
    {
        public string Comment { get; set; }

        public decimal Amount { get; set; }
    }

}