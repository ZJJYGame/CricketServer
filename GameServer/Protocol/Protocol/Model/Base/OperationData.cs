using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [Serializable]
    public class OperationData : IDisposable
    {
        public ushort OperationCode { get; set; }
        /// <summary>
        /// 若使用自定义数据类型，则建议使用dataContract；
        /// </summary>
        public IDataContract DataContract { get; set; }
        /// <summary>
        /// 若传输json对象或非自定义的数据类型，建议使用dataMessage;
        /// </summary>
        public object DataMessage { get; set; }
        public short ReturnCode { get; set; }
        /// <summary>
        /// 子操作码
        /// </summary>
        public short SubOperationCode { get; set; }
        public OperationData() { }
        public OperationData(ushort operationCode)
        {
            OperationCode = operationCode;
        }
        public virtual OperationData Clone()
        {
            return new OperationData()
            {
                DataContract = this.DataContract,
                OperationCode = this.OperationCode,
                ReturnCode = this.ReturnCode,
                DataMessage = this.DataMessage,
                SubOperationCode = this.SubOperationCode
            };
        }
        public virtual void Dispose()
        {
            OperationCode = 0;
            DataContract = null;
            ReturnCode = 0;
            DataMessage = null;
            SubOperationCode = 0;
        }
    }
}
