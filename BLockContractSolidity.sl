contract BlockContract{
	

	mapping(address => Transaction) public transactions;

	Struct Transaction{
		address seller;
		address buyer;
		address authority;
		address[] signatories
		address transactionId;
		address lockID

	}
	
	//Create a transaction and save it in the blockchain
	createTransaction(address buyer, address  authority, byte32[] signature, address lockID, address transcationID){
		//store these informations in the blockchain.
		transactions[transactionID] = Transaction(msg.sender, buyer, authority, signature, lockId, transcationId);

		//We will return always true in this step.
		return true;
	}

	//Verify if the user is eligible to open the lock (is the user a part of the transaction?)
	verifyUser(transactionID){

		Transaction tx = transactions[transactionID];

		if(tx.sellet == msg.sender || tx.buyer == msg.sender || tx.authority == msg.sender){
			return true;
		}else{
			return false;
		}

	}

}