using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Web3;
using Nethereum.Core.Signing.Crypto;
using System.Threading.Tasks;

namespace LockIxis.Ethereum
{
    public class LockAuthenticationService
    {
        public string PrivateKey { get; private set; }
        public string Url { get; private set; }
        private Web3 web3;
        private string address;
        private Contract contract;

        private string abi = "[{'constant':false,'inputs':[{'name':'score','type':'int256'}],'name':'setTopScore','outputs':[],'type':'function'},{'constant':true,'inputs':[{'name':'','type':'uint256'}],'name':'topScores','outputs':[{'name':'addr','type':'address'},{'name':'score','type':'int256'}],'type':'function'},{'constant':false,'inputs':[],'name':'getCountTopScores','outputs':[{'name':'','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'userTopScores','outputs':[{'name':'','type':'int256'}],'type':'function'}]";
        private string contractAddress = "0x2aa89fd00d26bc016d9939a1e9390a7a588cdfc6";

        private LockAuthenticationService(string privateKey, string url)
        {
            this.PrivateKey = privateKey;
            this.Url = url;
            this.web3 = new Web3(url);
            this.address = "0x" + EthECKey.GetPublicAddress(privateKey); //could do checksum
            this.contract = web3.Eth.GetContract(abi, contractAddress);
        }

        //public async Task<string> DeployContract(string )
        //{

        //    //return await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, LockIxisApp.CurrentUser().PublicKey);
        //}
    }
}
