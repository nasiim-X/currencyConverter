using System; //Provides basic functions (like input/output via Console).
using System.Net.Http; //Allows HTTP requests (to fetch currency rates).
using System.Threading.Tasks; //Enables asynchronous operations.
using Newtonsoft.Json; //Facilitates JSON data parsing.

class Program //The class is named Program.
{
    static async Task Main(string[] args) /*'static' because it doesn't require an instance to run.|
                                            'async Task' to enable asynchronous programming.|
                                            method name is 'Main', the entry point for the application.|
                                            Takes an array of strings (args) as input, allowing command-line arguments. */


    {
        Console.WriteLine("Currency Converter");

        // Step 1: Get the base currency from the user
        Console.Write("Enter the base currency (e.g., USD): ");
        string baseCurrency = Console.ReadLine().ToUpper(); //'.ToUpper()' to convert it to uppercase, ensuring case consistency 

        // Step 2: Get the target currency from the user
        Console.Write("Enter the target currency (e.g., EUR): ");
        string targetCurrency = Console.ReadLine().ToUpper();

        // Step 3: Get the amount to convert
        Console.Write("Enter the amount to convert: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount)) /*decimal.TryParse() to check if the input is a valid decimal number.|
                                                                        'decimal' is used for currency as it offers high precision.*/
        {
            Console.WriteLine("Invalid amount. Please enter a numeric value.");
            return;
        }

             //Try-Catch Block to Handle Conversion
        try //Attempts to perform the conversion.
        {
            // Step 4: Fetch conversion rate from an API
            decimal rate = await GetExchangeRate(baseCurrency, targetCurrency); //Calls the 'GetExchangeRate' method to fetch the rate.
            decimal convertedAmount = amount * rate;

            // Step 5: Display the result
            Console.WriteLine($"{amount} {baseCurrency} = {convertedAmount:F2} {targetCurrency}"); //Displays the converted amount, formatted to 2 decimal places using ':F2'.
        }
        catch (Exception ex)//Handles any errors that occur during the process (like network issues).
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task<decimal> GetExchangeRate(string baseCurrency, string targetCurrency) /*A helper method to get the exchange rate from the API.
                                                                                            Declared as 'async' since it involves network operations.*/
    {
            //Setting Up HTTP Client and Request
        using HttpClient client = new HttpClient(); //Creates an HTTP client to make requests.
        string url = $"https://api.exchangerate-api.com/v4/latest/{baseCurrency}"; //Constructs the API endpoint URL dynamically, replacing {baseCurrency} with the user input.

            //Sending the HTTP Request
        HttpResponseMessage response = await client.GetAsync(url); //Sends a GET request to the API.|Uses 'await' to wait for the response.
        response.EnsureSuccessStatusCode(); //EnsureSuccessStatusCode() checks if the request was successful --> If not, it throws an exception.

            //Reading and Parsing JSON Data
        string content = await response.Content.ReadAsStringAsync(); //Uses 'JsonConvert.DeserializeObject<dynamic>()' to parse the JSON into a dynamic object.
        var data = JsonConvert.DeserializeObject<dynamic>(content); //'dynamic' is used here because the structure of the JSON can vary.

            // Validating the Fetched Data
        if (data == null || data["rates"][targetCurrency] == null) //Checks if the parsed data is null or if the specified currency is not available.
        {
            throw new Exception("Currency not found."); //Throws an exception if the currency is not found.
        }

        return (decimal)data["rates"][targetCurrency];/*Accesses the nested JSON field to get the rate for the target currency.
                                                        Converts it to decimal before returning.*/
    }
}
