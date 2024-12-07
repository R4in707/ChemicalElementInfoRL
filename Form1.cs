//Creator: Rainier Lugue
//Class: CIS 022 



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace ChemicalElementInfoRL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        //Gets element Information from the api based on element name 
        async Task FetchElementInfo(string elementName)
        {
            using (HttpClient client = new HttpClient())
            {
                string formattedName = CapitalizeFirstLetter(elementName);
                string apiUrl = $"https://periodic-table-api.p.rapidapi.com/search?name={formattedName}";

                // API headers
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "761c3a09bemshf2efcb32a231a2bp17ef78jsn8c34203b2c67");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "periodic-table-api.p.rapidapi.com");

                try
                {// get request to API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // response status indicator for success
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Error: Unable to fetch data. Status code: {response.StatusCode}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //Deserializes JSON response with Newtonsoft.Json
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                    //Check if Valid data comes back
                    if (apiResponse?.Data == null || apiResponse.Data.Count == 0)
                    {
                        MessageBox.Show("No data found for the given element.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //display infomration of the first element returned
                    DisplayElementInfo(apiResponse.Data[0]);
                }
                catch (JsonException jsonEx) // Json deserialization error handler 
                {
                    MessageBox.Show($"JSON Deserialization Error: {jsonEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) //unexpected error handler 
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //displays fetched element info to the textboxes
        void DisplayElementInfo(Element element)
        {
            try
            {
                txtName.Text = element.name ?? "N/A";
                txtSymbol.Text = element.symbol ?? "N/A";
                txtAppearance.Text = element.appearance ?? "N/A";
                txtAtomicMass.Text = element.atomic_mass.ToString();
                txtBoil.Text = element.boil > 0 ? element.boil.ToString() : "N/A";
                txtCategory.Text = element.category ?? "N/A";
                txtDensity.Text = element.density > 0 ? element.density.ToString() : "N/A";
                txtDiscoveredBy.Text = element.discovered_by ?? "Unknown";
                txtMelt.Text = element.melt > 0 ? element.melt.ToString() : "N/A";
                txtMolarHeat.Text = element.molar_heat?.ToString() ?? "N/A";
                txtPhase.Text = element.phase ?? "N/A";
                txtSummary.Text = element.summary ?? "N/A";
                txtBlock.Text = element.block ?? "N/A";
                txtElectronConfig.Text = element.electron_configuration ?? "N/A";
                txtElectronegativity.Text = element.electronegativity_pauling?.ToString() ?? "N/A";
                txtIonizationEnergies.Text = element.ionization_energies != null
                    ? string.Join(", ", element.ionization_energies)
                    : "N/A";
            }
            catch (Exception ex) // hanlde expected error while displaying data
            {
                MessageBox.Show($"Error displaying element data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Captializes the first letter of the input string
        static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        // API's Response structure
        public class ApiResponse
        {
            public bool Success { get; set; }
            public int Results { get; set; }
            public List<Element> Data { get; set; }
        }

        //Represent an individual chemical element's data
        public class Element
        {
            public string name { get; set; }
            public string appearance { get; set; }
            public double atomic_mass { get; set; }
            public double boil { get; set; }
            public string category { get; set; }
            public double density { get; set; }
            public string discovered_by { get; set; }
            public double melt { get; set; }
            public double? molar_heat { get; set; }
            public string phase { get; set; }
            public string summary { get; set; }
            public string symbol { get; set; }
            public string electron_configuration { get; set; }
            public double? electronegativity_pauling { get; set; }
            public List<double> ionization_energies { get; set; }
            public string block { get; set; }
        }

        // Clears all text boxes to reset form
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtElement.Text = string.Empty;

            txtName.Clear();
            txtSymbol.Clear();
            txtAppearance.Clear();
            txtAtomicMass.Clear();
            txtBoil.Clear();
            txtCategory.Clear();
            txtDensity.Clear();
            txtDiscoveredBy.Clear();
            txtMelt.Clear();
            txtMolarHeat.Clear();
            txtPhase.Clear();
            txtSummary.Clear();
            txtBlock.Clear();
            txtElectronConfig.Clear();
            txtElectronegativity.Clear();
            txtIonizationEnergies.Clear();
        }

        //Exit app
        private void btnExit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        // search button that initiaes the element info search
        private async void btnSearch_Click_1(object sender, EventArgs e)
        {

            string elementName = txtElement.Text.Trim();

            if (string.IsNullOrWhiteSpace(elementName)) // check for valid input
            {
                MessageBox.Show("Invalid input. Please enter a valid element name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await FetchElementInfo(elementName); //fetches and displays element info
            }
            catch (Exception ex) // handle unexpected errors during the search
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
    }
}
