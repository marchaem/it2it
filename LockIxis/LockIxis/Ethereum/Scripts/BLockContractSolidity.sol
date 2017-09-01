pragma solidity ^0.4.10;

contract BlockContract {
    address private _authority; //the authority can only be accessed internally from the contract

    struct BlockTransaction {
        address sender; //sender of the transaction
        address _user;   //list of authorized users
        address _lockId;  //Id of the lock, there is only one for now
        address _transactionId;  //Id of the transaction
        bool _isOpen;  //Status of the lock (closed or opened)
        string _status;  //Status of the transaction(action done)
    }

    event updateTransaction(address _user, address _lockId, address _transactionId, bool _isOpen, string _status);
    mapping (address => BlockTransaction) transactions;
    uint transactionCount=0;  //counts the number of transactions triggered
    BlockTransaction[] transactionArray; //array that stores all transactions

    function BlockContract() {
        _authority = msg.sender;  //the creator of the contract is the authority
    }

    modifier isAuthority() {
        if (msg.sender != _authority) throw;
        else{
            _;
        }
    }

    modifier isUserOrAuthority(address transactionId) {
        if (msg.sender != _authority || msg.sender != transactions[transactionId]._user) throw;
        else {
            _;
        }
    }


    function createTransaction(address user, address lockId, address transactionId, bool isOpen, string status)  internal returns(bool saved){
        var block = BlockTransaction(msg.sender,user,lockId,transactionId,isOpen,status);
        transactions[transactionId] = block;
        transactionArray.push(block);
        transactionCount++;
        return true;
    }

    function removeTransaction(address transactionId) returns(bool removed) { //we delete a transaction from the mapping
        delete transactions[transactionId];
        transactionCount--;
        return true;
    }

    //function to open the lock by the authority or auhtorized user
    function openLock(address user,address transactionId,address lockId) isUserOrAuthority(transactionId)  returns(bool opened) {
        if(user == _authority) {
            createTransaction(user,lockId,transactionId ,true,"LOCK OPENED BY AUTHORITY");
        }else {       
            createTransaction(user,lockId,transactionId,true,"LOCK OPENED BY USER :");
        }       
        return true;
    }

    function addUser(address user,address transactionId,address lockId,bool lockStatus) isAuthority()  returns(bool added) {
        if(lockId == 0x0000) throw;
        createTransaction(user,transactionId,lockId,lockStatus,"USER ADDED ");
    }

    function addLock(address transactionId,address lockId) isAuthority()  returns (bool added) {
        createTransaction(0x0000,transactionId,lockId,true,"LOCK ADDED"); //locks added are always open at the beginning, and there are no users attached
    }

    function closeLock(address user,address transactionId,address lockId) isUserOrAuthority(transactionId) returns(bool closed) {
        createTransaction(user,transactionId,lockId,false,"LOCK CLOSED");
        return true;
    }

    function getLockStatus(address transactionId) constant returns (bool status) {
        return transactions[transactionId]._isOpen;
    
    }

    function getUser(address transactionId) constant returns (address user) {
        return transactions[transactionId]._user;
    }

    function getLockId(address transactionId) constant returns (address lock) {
        return transactions[transactionId]._lockId;
    }

    function getStatus(address transactionId) constant returns(string status) {
        return transactions[transactionId]._status;
    }

    function getBlockTransaction(address transactionId) constant private returns (BlockTransaction block) {
        return transactions[transactionId];
    }

    function nbrTransanction() constant returns (uint count) {  //returns the number of transactions
        return transactionCount;
    }

    //hand-made toString() for an address
    function toString(address user) returns(string) {
        bytes memory b = new bytes(20);
        for (uint i = 0; i < 20; i++) {
            b[i] = byte(uint8(uint(user)/(2**(8*(19.0-i)))));
        }
        return string(b);
    }


}