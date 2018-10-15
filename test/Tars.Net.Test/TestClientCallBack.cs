using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Metadata;

namespace Tars.Net.Test
{
    internal class TestClientCallBack : IClientCallBack
    {
        private TestTarsConvert testTarsConvert;

        public TestClientCallBack(TestTarsConvert testTarsConvert)
        {
            this.testTarsConvert = testTarsConvert;
        }

        public void CallBack(Response msg)
        {
            throw new System.NotImplementedException();
        }

        public (string servantName, string funcName)? FindRpcMethod(int callBackId)
        {
            return testTarsConvert.FindRpcMethod(callBackId);
        }

        public int NewCallBackId()
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> NewCallBackTask(int id, int timeout, string servantName, string funcName)
        {
            throw new System.NotImplementedException();
        }
    }
}