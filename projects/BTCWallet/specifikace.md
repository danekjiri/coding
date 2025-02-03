## Specifikace C# projektu leto (NPRG035, NPRG038, NPRG064, NPRG057)

### BTC software wallet using Avalonia UI Framework (desktop)

#### Motivace

Bitcoinove penezky jsou klicovym nastrojem proj spravu a bezpecne uchovani
kryptomen. Sprava jejich adres ci tvorba novych transakci je pro netechnickeho
cloveka komplikovana, a proto je cilem projektu navrhnout intuitivni, bezpecnou
a flexibilni aplikaci resici zminene problemy a integraci uzivatele do btc
site.

#### Pouzite knihovny

1. NBitcoin (https://github.com/MetacoSA/NBitcoin) - wrapper nad zakladnimi Bitcoin primitivy implementujici jednotlive BIP*
2. IronQR (https://www.nuget.org/packages/IronQR/2024.4.1) - import/export klicu a adres
3. EF SQLite (https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite)

#### Pouzite frameworky

1. Avalonia UI (https://avaloniaui.net/Platforms)

#### Funkcionality

- Logicke oddeleni penezenek na pojmenovane ucty
- Moznost heslovani jednotlivych uctu
- Importovani/Exportovani/Tvorba jednotlivych privatnich klicu
- Importovani/Exportovani/Tvorba HD-wallet (BIP32, BIP39)
- Importovani/Exportovani readonly penezenky (xpub)
- Modifikovatelna tvorba transakce (UTXO, fees, ...)
- Detailni zobrazeni transakce
- Obdrzeni transakce (zobrazeni vlastni adresy)
- Hlavni stranka se zakladnimi udaji a kurzy vuci statnim fiat menam (USD, EUR, ...)
- Moznost pripojeni vlastniho full-node clienta
- Komunikace s API pripojenym full-node clientem pomoci RPC (JSON)
- (De)Serializace a prace s vracenymi JSON daty od full-node clienta
- Ulozeni dat: adresy, klice, UTXOs, ... v EF SQLite databazi
