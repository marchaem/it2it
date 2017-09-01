using System;
using System.Text;
using Nethereum.Util;
using Nethereum.Hex.HexTypes;
using Nethereum.Geth;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Signer;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace LockIxis.Ethereum
{

    public class LockAuthenticationService
    {
        public string PrivateKey { get; private set; }
        public string Url { get; private set; }
        private string password;
        private string bytecode;
        private readonly Web3Geth web3;
        private string senderAddress;  //the sender's address
        private Contract contract;     //Our smart contract   
        //the abi comes from our compiled solidity file
        private string abi = @"[{""constant"":false,""inputs"":[{""name"":""transactionId"",""type"":""address""}],""name"":""removeTransaction"",""outputs"":[{""name"":""removed"",""type"":""bool""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[{""name"":""transactionId"",""type"":""address""}],""name"":""getLockStatus"",""outputs"":[{""name"":""status"",""type"":""bool""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[{""name"":""transactionId"",""type"":""address""}],""name"":""getStatus"",""outputs"":[{""name"":""status"",""type"":""string""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""user"",""type"":""address""}],""name"":""toString"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""user"",""type"":""address""},{""name"":""transactionId"",""type"":""address""},{""name"":""lockId"",""type"":""address""},{""name"":""lockStatus"",""type"":""bool""}],""name"":""addUser"",""outputs"":[{""name"":""added"",""type"":""bool""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[{""name"":""transactionId"",""type"":""address""}],""name"":""getUser"",""outputs"":[{""name"":""user"",""type"":""address""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""user"",""type"":""address""},{""name"":""transactionId"",""type"":""address""},{""name"":""lockId"",""type"":""address""}],""name"":""closeLock"",""outputs"":[{""name"":""closed"",""type"":""bool""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""user"",""type"":""address""},{""name"":""transactionId"",""type"":""address""},{""name"":""lockId"",""type"":""address""}],""name"":""openLock"",""outputs"":[{""name"":""opened"",""type"":""bool""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[{""name"":""transactionId"",""type"":""address""}],""name"":""getLockId"",""outputs"":[{""name"":""lock"",""type"":""address""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""transactionId"",""type"":""address""},{""name"":""lockId"",""type"":""address""}],""name"":""addLock"",""outputs"":[{""name"":""added"",""type"":""bool""}],""payable"":false,""type"":""function""},{""inputs"":[],""payable"":false,""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""_user"",""type"":""address""},{""indexed"":false,""name"":""_lockId"",""type"":""address""},{""indexed"":false,""name"":""_transactionId"",""type"":""address""},{""indexed"":false,""name"":""_isOpen"",""type"":""bool""},{""indexed"":false,""name"":""_status"",""type"":""string""}],""name"":""updateTransaction"",""type"":""event""}]";
        public LockAuthenticationService(string pass, string privateKey, string url, string bcode)
        {
            password = pass;
            Url = url;
            web3 = new Web3Geth(url);
            PrivateKey = privateKey;
            senderAddress = EthECKey.GetPublicAddress(PrivateKey);
            Console.WriteLine("notre adresse publique est :" + senderAddress);
            bytecode = "0x" + bcode;
        }

        //we unlock the account
        public async Task unlock()  
        {
            Console.WriteLine("on entre dans le unlock");
            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, new HexBigInteger(600000)); //the quantity of gas used is maxed compared to the estimated gas required, since the over-due will be replenished in or account
            Console.WriteLine("le unlock vaut " + unlockResult);
            Console.WriteLine("on termine le unlock");
        }

        public async Task<string> getTransactionHash()
        {
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, bytecode, senderAddress, new HexBigInteger(900000));
            return transactionHash;
        }

        //we retrieve the contract with our parameters (abi, bytecode and senderAddress)
        public async Task getContract()
        {
            Console.WriteLine("on entre dans le getContract");
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, bytecode, senderAddress, new HexBigInteger(900000));
            Console.WriteLine("la transactionHash vaut " + transactionHash);
            var receipt = await MineAndGetReceiptAsync(web3, transactionHash);
            Console.WriteLine("la receipt vaut " + receipt.TransactionHash);
            var contractAddress = receipt.ContractAddress;
            contract = web3.Eth.GetContract(abi, contractAddress);
            Console.WriteLine("le contract vaut " + contract.GetType().ToString());
            Console.WriteLine("on sort du getContrat");
        }

        //this function Mines the contract and retrieves the receipt of the contract containing the main infos of the contract (such as its address on the blockchain and its functions)
        public async Task<TransactionReceipt> MineAndGetReceiptAsync(Web3Geth web3, string transactionHash)
        {
            Console.WriteLine("on mine...");
            var miningResult = await web3.Miner.Start.SendRequestAsync(6);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            if (transactionHash == null)
            {
                Console.WriteLine("le transactionHash est devenue nulle");
            }
            while (receipt == null)
            {

                Console.WriteLine("receipt est nulle...");
                await Task.Delay(5000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }
            Console.WriteLine("notre receipt n'est plus nulle");
            if (receipt.ContractAddress != null)
            {
                var code = await web3.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);
                if (String.IsNullOrEmpty(code))
                {
                    throw new Exception("Code was not deployed correctly, verify bytecode or enough gas was to deploy the contract");
                }
            }
            //we stop the miner 
            miningResult = await web3.Miner.Stop.SendRequestAsync();
            Console.WriteLine("on a terminé le minage");
            return receipt;
        }

        //returns the function from the contract with its name as an input, the url has to be the same as in the solidity file 
        public Function getFunction(string url)
        {
            Console.WriteLine("on rentre dans le getFunction avec " + url);
            return contract.GetFunction(url);
        }


        //adds an user in our blockchain
        public async Task<TransactionReceipt> addUser(string user, string transactionId, string lockId, bool lockStatus)
        {
            Console.WriteLine("on rentre dans le addUser");
            Console.WriteLine("l'user que l'on veut ajouté vaut " + user);
            AddressUtil adressUtil = new AddressUtil();  //utility to convert our public key to eth address
            string hashUser = adressUtil.ConvertToChecksumAddress(user);  //hashage d'une adresse publique en adresse ethereum
            hashUser = adressUtil.ConvertToValid20ByteAddress(hashUser);
            string hashTransactionId = adressUtil.ConvertToChecksumAddress(transactionId);
            hashTransactionId = adressUtil.ConvertToValid20ByteAddress(hashTransactionId);
            string hashLockId = adressUtil.ConvertToChecksumAddress(lockId);
            hashLockId = adressUtil.ConvertToValid20ByteAddress(hashLockId);
            if (!adressUtil.IsChecksumAddress(hashUser))
            {
                Console.WriteLine("on a pas une bonne adresse checkSum");
            }
            Console.WriteLine("notre adresse finale de l'user est : " + hashUser);
            Console.WriteLine("on rentre dans le getFunction de addUser");
            Function addUser = getFunction("addUser");
            var result = await addUser.SendTransactionAsync(senderAddress, new HexBigInteger(900000), null, hashUser, hashTransactionId, hashLockId, lockStatus); //we call addUser from the solidity code
            Console.WriteLine("on a ajouté l'user : " + hashUser + " et le trasactionId vaut " + hashTransactionId + " et le lock associé vaut : " + hashLockId);
            while (result == null)
            {
                Console.WriteLine("notre adduser.sendTransationAsync est nulle...");
                await Task.Delay(5000);
                result = await addUser.SendTransactionAsync(senderAddress, new HexBigInteger(900000), null, hashUser, hashTransactionId, hashLockId, true);
            }
            var receipt = await MineAndGetReceiptAsync(web3, result);
            Console.WriteLine("on sort du addUser");
            return receipt;
        }

        //retrieves the user we added with addUser
        public async Task<string> getUser(string transactionId)
        {
            Console.WriteLine("on entre dans returnUser");
            Function returnUser = getFunction("getUser");
            var result = await returnUser.CallAsync<string>(transactionId);
            Console.WriteLine("l'id stocké vaut :" + result);
            return result;
        }

        //adds a lock the same way as for a user, both are ethereum addresses
        public async Task<string> addLock(string transactionId, string lockId)
        {
            Console.WriteLine("on rentre dans le addLock");
            //Console.WriteLine("le lock que l'on veut ajouté vaut " + lockId);
            AddressUtil adressUtil = new AddressUtil();  //utility to convert our public key to eth address
            string hashTransactionId = adressUtil.ConvertToChecksumAddress(transactionId);
            hashTransactionId = adressUtil.ConvertToValid20ByteAddress(hashTransactionId);
            string hashLockId = adressUtil.ConvertToChecksumAddress(lockId);
            hashLockId = adressUtil.ConvertToValid20ByteAddress(hashLockId);
            if (!adressUtil.IsChecksumAddress(hashLockId))
            {
                Console.WriteLine("on a pas une bonne adresse checkSum");
            }

            if (!adressUtil.IsChecksumAddress(hashLockId))
            {
                Console.WriteLine("on a pas une bonne adresse checkSum");
            }
            //Console.WriteLine("notre adresse finale du lock est : " + hashLockId);

            Console.WriteLine("on rentre dans le getFunction de addLock");
            Function addLock = getFunction("addLock");
            var result = await addLock.SendTransactionAsync(senderAddress, new HexBigInteger(900000), null,hashLockId);
            while (result == null)
            {
                Console.WriteLine("notre adduser.sendTransationAsync est nulle...");
                await Task.Delay(5000);
                result = await addLock.SendTransactionAsync(senderAddress, new HexBigInteger(900000), null,hashLockId);
            }
            var receipt = await MineAndGetReceiptAsync(web3, result);
           // Console.WriteLine("on a ajouté le lock " + hashLockId + " et le transactionId vaut :" + transactionId);
            Console.WriteLine("on sort du addLock");
            return result;
        }

        public async Task<string> getLock(string transactionId)
        {
            Console.WriteLine("on entre dans le getLock");
            Function getLockId = getFunction("getLockId");
            var result = await getLockId.CallAsync<string>(transactionId);
            Console.WriteLine("le lock stocké vaut :" + result);
            return result;
        }

        //changes a bool to false for the lockStatus
        public async Task closeLock(string user, string transactionId, string lockId)
        {

            Console.WriteLine("on rentre dans closeLock");
            Console.WriteLine("le lock que l'on veut fermé vaut " + lockId);
            AddressUtil adressUtil = new AddressUtil();  //utility to convert our public key to eth address
            string hashUser = adressUtil.ConvertToChecksumAddress(user);  //hashage d'une adresse publique en adresse ethereum
            hashUser = adressUtil.ConvertToValid20ByteAddress(hashUser);
            string hashTransactionId = adressUtil.ConvertToChecksumAddress(transactionId);
            hashTransactionId = adressUtil.ConvertToValid20ByteAddress(hashUser);
            string hashLockId = adressUtil.ConvertToChecksumAddress(lockId);
            hashLockId = adressUtil.ConvertToValid20ByteAddress(hashLockId);
            Function closeLock = contract.GetFunction("closeLock");
            var gas = await closeLock.EstimateGasAsync();
            var result = await closeLock.SendTransactionAsync(senderAddress, new HexBigInteger(900000), null, hashUser, hashTransactionId, hashLockId);
            Console.WriteLine("on a effectué la transaction du closeLock, elle vaut " + result + " et la transactionId vaut :" + hashTransactionId);
            while (result == null)
            {
                Console.WriteLine("la transaction du closeLock est nulle");
                await Task.Delay(5000);
                result = await closeLock.SendTransactionAsync(senderAddress, new HexBigInteger(900000), null);

            }
            var receipt = await MineAndGetReceiptAsync(web3, result);
            /*Console.WriteLine("on rentre dans le GetEvent du contrat");
            var lockChangedEvent = contract.GetEvent("lockChanged");

            Console.WriteLine("on rentre dans le CreateFilterAsync du contrat");
            var filterAllContract = await contract.CreateFilterAsync();
            var filterAll = await lockChangedEvent.CreateFilterAsync();
            Console.WriteLine("on rentre dans le getEventLog");
            var closeLockEventLog = contract.GetEvent("lockChangedLog");
            var filterAllLog = await closeLockEventLog.CreateFilterAsync();*/




            Console.WriteLine("on sort du closeLock");
        }

        //retrieves the status of the lock
        public async Task<bool> getLockStatus()
        {
            Console.WriteLine("on rentre dans le getStatus");
            Function getLockStatus = getFunction("getLockStatus");
            var result = await getLockStatus.CallAsync<bool>();
            Console.WriteLine("on sort du getLockStatus et le lock vaut " + result.ToString() + " et il doit valoir false");
            return result;
        }

        //gets the status of the transaction (what's happening)
        public async Task<string> getTransactionStatus(string transactionId)
        {
            Console.WriteLine("on rentre dans le geTransactionStatus");
            Function getTransactionStatus = getFunction("getStatus");
            var result = await getTransactionStatus.CallAsync<string>(transactionId);
            Console.WriteLine("on sort du getTransactionStatus et le statut vaut :" + result);
            return result;
        }

        //retrieves the number of transactions generated
        public async Task<uint> getTransactionCount()
        {
            Console.WriteLine("on rentre dans getTransactionCount");
            Function getTransactionCount = getFunction("nbrTransaction");
            var result = await getTransactionCount.CallAsync<uint>();
            Console.WriteLine("on sort du getTransactionCount et il y a en tout " + result + " transactions");
            return result;
        }

        //verifies if a user is authorized
        public async Task<bool> verifyUser(string user)
        {
            Console.WriteLine("on rentre dans le verifyUser");
            Function verifyuser = getFunction("verifyUser");
            var result = await verifyuser.CallAsync<bool>(user);
            Console.WriteLine("on sort de verifyUser");
            return result;
        }

        //updates a transactionId
        public string updateTransactionId(string transactionId)
        {
            byte[] toBytes = Encoding.ASCII.GetBytes(transactionId);
            toBytes[toBytes.Length - 1]++;
            transactionId = Encoding.ASCII.GetString(toBytes);
            return transactionId;
        }

    }

    public class blockTransaction
    {
        [Parameter("address", "_sender", 1)]
        public string _sender { get; set; }
        [Parameter("address", "_user", 2)]
        public string _user { get; set; }
        [Parameter("address", "_transactionId", 3)]
        public string _transactionId { get; set; }
        [Parameter("address", "_lockId", 4)]
        public string _address { get; set; }
        [Parameter("address", "_signedAddresse",5)]
        public string _signedAddresse { get; set; }
        [Parameter("bool", "_isOpen", 6)]
        public string _isOpen { get; set; }
        [Parameter("string", "_status", 7)]
        public string _status { get; set; }

    }

    public class locked
    {
        [Parameter("address", "_authorized", 1)]
        public string user { get; set; }
    }

    public class StoredEvent
    {
        [Parameter("bool", "a", 1, true)]
        public bool Initial { get; set; }
        [Parameter("bool", "result", 3, true)]
        public bool Final { get; set; }
    }

    public class StoredEventSenderLog
    {
        [Parameter("bool", "a", 1, true)]
        public bool Initial { get; set; }
        [Parameter("bool", "result", 2, true)]
        public bool Final { get; set; }
        [Parameter("string", "hello", 3, true)]
        public string Hello { get; set; }
        [Parameter("address", "sender", 4, false)]
        public bool Sender { get; set; }
    }
}
