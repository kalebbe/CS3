/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
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
        //Constructor takes status, message, and data and assigns them.
        public DTO(int Status, string Message, List<string> Data)
        {
            this.Status = Status;
            this.Message = Message;
            this.Data = Data;
        }
        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<string> Data { get; set; }
    }
}