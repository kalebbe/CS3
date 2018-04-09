/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      DTO.cs
 * @revision:  1.0
 * @synapsis:  This class will be used in the future for error messaging through the api.
 */

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StatsService
{
    [DataContract]
    public class DTO
    {
        public DTO(int ErrorCode, string ErrorMsg, List<string> Data)
        {
            this.ErrorCode = ErrorCode;
            this.ErrorMessage = ErrorMsg;
            this.Data = Data;
        }
        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public List<string> Data { get; set; }
    }
}