//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomJobs
{
    using System;
    using System.Collections.Generic;
    
    public partial class IPLogs
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public string Client { get; set; }
        public System.DateTime DateTime { get; set; }
        public int MemberId { get; set; }
    
        public virtual Members Members { get; set; }
    }
}
