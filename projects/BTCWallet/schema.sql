BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "PublicKeys" (
                                             "PublicKeyID"	INTEGER,
                                             "PublicKeyWif"	TEXT NOT NULL,
                                             "PrivateKeyWif"	TEXT,
                                             "ChainCode"    TEXT NOT NULL,
                                             "IsCompressed" INTEGER NOT NULL,
                                             "SequenceNumber"	INTEGER NOT NULL,
                                             "WalletID"	INTEGER,
                                             PRIMARY KEY("PublicKeyID" AUTOINCREMENT),
    FOREIGN KEY("WalletID") REFERENCES "Wallets"("WalletID")
    );
CREATE TABLE IF NOT EXISTS "Wallets" (
                                         "WalletID"	INTEGER,
                                         "Mnemonic"	TEXT,
                                         "Derivation"	TEXT,
                                         "MasterKeyWif"	TEXT,
                                         "Passphrase" TEXT,
                                         "XPubWif"	TEXT NOT NULL,
                                         "IsReadOnly" INTEGER NOT NULL,
                                         PRIMARY KEY("WalletID" AUTOINCREMENT)
    );
CREATE TABLE IF NOT EXISTS "Profiles" (
                                          "ProfileID"	INTEGER,
                                          "Username"	TEXT NOT NULL,
                                          "PasswordHash"	TEXT NOT NULL,
                                          "LastUnusedAddrIndexRec" INTEGER NOT NULL,
                                          "LastUnusedAddrIndexChan" INTEGER NOT NULL,
                                          "WalletID"	INTEGER,
                                          PRIMARY KEY("ProfileID" AUTOINCREMENT),
    FOREIGN KEY("WalletID") REFERENCES "Wallets"("WalletID")
    );
CREATE TABLE IF NOT EXISTS "Addresses" (
                                           "AddressID"	INTEGER,
                                           "AddressWif"	TEXT NOT NULL,
                                           "PublicKeyID" INTEGER,
                                           "IsChange"	INTEGER NOT NULL,
                                           "Network"	TEXT NOT NULL,
                                           PRIMARY KEY("AddressID" AUTOINCREMENT),
     FOREIGN KEY("PublicKeyID") REFERENCES "PublicKeys"("PublicKeyID")
    );
CREATE TABLE IF NOT EXISTS "Transactions" (
                                              "TransactionID"	INTEGER,
                                              "TxHash"	TEXT NOT NULL,
                                              "CreatedAt"	TEXT NOT NULL,
                                              "Comment"	TEXT,
                                              "IsCreated" INTEGER NOT NULL,
                                              "BlockHeight" INTEGER,
                                              "FeeSatoshi"  INTEGER,
                                              PRIMARY KEY("TransactionID" AUTOINCREMENT)
    );
CREATE TABLE IF NOT EXISTS "Inputs" (
                                        "InputID"	INTEGER,
                                        "TransactionID"	INTEGER NOT NULL,
                                        "InputIndex"	INTEGER NOT NULL,
                                        "AmountSatoshi"	INTEGER NOT NULL,
                                        "ScriptSig"	TEXT,
                                        "PreviousOutTxHash" TEXT NOT NULL,
                                        "PreviousOutIndex" INTEGER NOT NULL,
                                        PRIMARY KEY("InputID" AUTOINCREMENT),
    FOREIGN KEY("TransactionID") REFERENCES "Transactions"("TransactionID")
    );
CREATE TABLE IF NOT EXISTS "Outputs" (
                                         "OutputID"	INTEGER,
                                         "TransactionID"    INTEGER NOT NULL,
                                         "AddressID"   INTEGER,
                                         "AddressWif"   TEXT NOT NULL,
                                         "AmountSatoshi"    INTEGER NOT NULL,
                                         "OutputIndex"	INTEGER NOT NULL,
                                         "ScriptPubKey"	TEXT NOT NULL,
                                         "IsSpent" INTEGER,
                                         "InputID" INTEGER,
                                         PRIMARY KEY("OutputID" AUTOINCREMENT),
    FOREIGN KEY("TransactionID") REFERENCES "Transactions"("TransactionID"),
    FOREIGN KEY("AddressID") REFERENCES "Addresses"("AddressID"),
    FOREIGN KEY("InputID") REFERENCES "Inputs"("InputID")
    );
