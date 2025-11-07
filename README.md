# BankOscar
Jag byggde den här applikationen som ett litet men komplett bank-system där användargränssnittet hålls enkelt och all logik ligger i domän- och service-lagret. Den centrala klassen är BankAccount, som kapslar in tillståndsförändringar (Deposit, Withdraw, TransferTo) och automatiskt registrerar en oföränderlig lista av Transaction-objekt efter varje operation. Genom att hålla transaktionslistan privat och endast exponera den via GetTransactions() kan gränssnittet läsa historik utan att kunna ändra den – tydlig separation och färre fel.

Jag införde interfaces (IAccountService, IStorageService m.fl.) och kopplade ihop dem med Dependency Injection i Program.cs. Fördelen är utbytbarhet och testbarhet: i dag används ett in-memory-/localStorage-lager, men det kan ersättas med en databas som EF Core utan att koden i sidorna behöver ändras. Gränssnittet pratar mot abstraktioner i stället för konkreta klasser. Varje asynkron metod i AccountService fungerar som ett tunt lager ovanpå domänlogiken och anropar sedan SaveAsync(), vilket gör koden lätt att läsa och säkerställer att alla sidoeffekter sker på samma ställe.

Validering sker där den hör hemma. Domänlagret skyddar mot ogiltiga värden (belopp > 0, inga övertrasserade uttag) och användargränssnittet visar begripliga felmeddelanden i stället för tekniska undantag. Den uppdelningen gör det svårt att av misstag kringgå regler om man senare lägger till fler sidor.

Jag lade också till några praktiska funktioner – till exempel förhandsvisning och applicering av ränta på startsidan samt en Budget-sida med kategorier och utgifter som automatiskt dras från valt konto. Dessa visar hur nya funktioner kan använda samma domänlogik utan att duplicera kod.

För enkel åtkomst är webbplatsen skyddad med en fyrsiffrig PIN-kod. Inloggningssidan använder en enkel IAuthService som bara kontrollerar koden och håller koll på autentiseringsstatus. Koden är 1234, så att man snabbt kan testa applikationen.

Jag valde Blazor för att snabbt kunna iterera och bygga komponentiserade gränssnitt. Sidor som Transaction History sammanställer alla kontons transaktioner i en gemensam tabell med filtrering, vilket visar att domänlagret är den enda sanningskällan. Stilen hålls avsiktligt enkel (Bootstrap) för att fokus ska ligga på funktionaliteten.

Målet var att skapa ett “walking skeleton”: först få ett fungerande flöde (skapa konto, insättning/uttag, överföring, historik), därefter bygga vidare med budget och ränta, och slutligen lägga till databas, loggning och tester – utan att behöva ändra strukturen tack vare mina interfaces och DI-lösning.


