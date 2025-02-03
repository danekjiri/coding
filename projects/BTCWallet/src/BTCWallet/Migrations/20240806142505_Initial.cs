using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTCWallet.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TxHash = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    IsCreated = table.Column<long>(type: "INTEGER", nullable: false),
                    BlockHeight = table.Column<long>(type: "INTEGER", nullable: true),
                    FeeSatoshi = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    WalletID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mnemonic = table.Column<string>(type: "TEXT", nullable: true),
                    Derivation = table.Column<string>(type: "TEXT", nullable: true),
                    MasterKeyWif = table.Column<string>(type: "TEXT", nullable: true),
                    Passphrase = table.Column<string>(type: "TEXT", nullable: true),
                    XPubWif = table.Column<string>(type: "TEXT", nullable: false),
                    IsReadOnly = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.WalletID);
                });

            migrationBuilder.CreateTable(
                name: "Inputs",
                columns: table => new
                {
                    InputID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionID = table.Column<long>(type: "INTEGER", nullable: false),
                    InputIndex = table.Column<long>(type: "INTEGER", nullable: false),
                    AmountSatoshi = table.Column<long>(type: "INTEGER", nullable: false),
                    ScriptSig = table.Column<string>(type: "TEXT", nullable: true),
                    PreviousOutTxHash = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousOutIndex = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inputs", x => x.InputID);
                    table.ForeignKey(
                        name: "FK_Inputs_Transactions_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "Transactions",
                        principalColumn: "TransactionID");
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    ProfileID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    LastUnusedAddrIndexRec = table.Column<long>(type: "INTEGER", nullable: false),
                    LastUnusedAddrIndexChan = table.Column<long>(type: "INTEGER", nullable: false),
                    WalletID = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.ProfileID);
                    table.ForeignKey(
                        name: "FK_Profiles_Wallets_WalletID",
                        column: x => x.WalletID,
                        principalTable: "Wallets",
                        principalColumn: "WalletID");
                });

            migrationBuilder.CreateTable(
                name: "PublicKeys",
                columns: table => new
                {
                    PublicKeyID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublicKeyWif = table.Column<string>(type: "TEXT", nullable: false),
                    PrivateKeyWif = table.Column<string>(type: "TEXT", nullable: true),
                    ChainCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsCompressed = table.Column<long>(type: "INTEGER", nullable: false),
                    SequenceNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    WalletID = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicKeys", x => x.PublicKeyID);
                    table.ForeignKey(
                        name: "FK_PublicKeys_Wallets_WalletID",
                        column: x => x.WalletID,
                        principalTable: "Wallets",
                        principalColumn: "WalletID");
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressWif = table.Column<string>(type: "TEXT", nullable: false),
                    PublicKeyID = table.Column<long>(type: "INTEGER", nullable: true),
                    IsChange = table.Column<long>(type: "INTEGER", nullable: false),
                    Network = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_Addresses_PublicKeys_PublicKeyID",
                        column: x => x.PublicKeyID,
                        principalTable: "PublicKeys",
                        principalColumn: "PublicKeyID");
                });

            migrationBuilder.CreateTable(
                name: "Outputs",
                columns: table => new
                {
                    OutputID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionID = table.Column<long>(type: "INTEGER", nullable: false),
                    AddressID = table.Column<long>(type: "INTEGER", nullable: true),
                    AddressWif = table.Column<string>(type: "TEXT", nullable: false),
                    AmountSatoshi = table.Column<long>(type: "INTEGER", nullable: false),
                    OutputIndex = table.Column<long>(type: "INTEGER", nullable: false),
                    ScriptPubKey = table.Column<string>(type: "TEXT", nullable: false),
                    IsSpent = table.Column<long>(type: "INTEGER", nullable: true),
                    InputID = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outputs", x => x.OutputID);
                    table.ForeignKey(
                        name: "FK_Outputs_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_Outputs_Inputs_InputID",
                        column: x => x.InputID,
                        principalTable: "Inputs",
                        principalColumn: "InputID");
                    table.ForeignKey(
                        name: "FK_Outputs_Transactions_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "Transactions",
                        principalColumn: "TransactionID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PublicKeyID",
                table: "Addresses",
                column: "PublicKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_TransactionID",
                table: "Inputs",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Outputs_AddressID",
                table: "Outputs",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Outputs_InputID",
                table: "Outputs",
                column: "InputID");

            migrationBuilder.CreateIndex(
                name: "IX_Outputs_TransactionID",
                table: "Outputs",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_WalletID",
                table: "Profiles",
                column: "WalletID");

            migrationBuilder.CreateIndex(
                name: "IX_PublicKeys_WalletID",
                table: "PublicKeys",
                column: "WalletID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Outputs");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Inputs");

            migrationBuilder.DropTable(
                name: "PublicKeys");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
