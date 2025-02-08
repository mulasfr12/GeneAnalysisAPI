using AngleSharp;
using GeneAnalysisAPI.Models;
using GeneAnalysisAPI.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeneAnalysisAPI.Services
{
    public class ScraperService
    {
        private readonly IGeneRepository _geneRepository;
        private readonly HashSet<string> _cGasStingGenes = new()
        {
            "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9", "CXCL11",
            "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM", "IL6", "IL8"
        };

        public ScraperService(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository;
        }
        public async Task ProcessClinicalTsvFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Clinical TSV file not found", filePath);

            var clinicalDataList = new List<ClinicalData>();

            using var reader = new StreamReader(filePath);
            var headers = reader.ReadLine()?.Split('\t')
                .Select(h => h.Trim())
                .ToArray();

            if (headers == null || !headers.Contains("bcr_patient_barcode"))
                throw new Exception("Invalid Clinical TSV format: Missing 'bcr_patient_barcode' column.");

            int patientIdIndex = Array.IndexOf(headers, "bcr_patient_barcode");
            int dssIndex = Array.IndexOf(headers, "DSS");
            int osIndex = Array.IndexOf(headers, "OS");
            int clinicalStageIndex = Array.IndexOf(headers, "clinical_stage");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Split('\t');
                if (line == null || line.Length <= clinicalStageIndex) continue;

                clinicalDataList.Add(new ClinicalData
                {
                    PatientId = line[patientIdIndex].Trim(),
                    DiseaseSpecificSurvival = int.Parse(line[dssIndex]),
                    OverallSurvival = int.Parse(line[osIndex]),
                    ClinicalStage = line[clinicalStageIndex].Trim()
                });
            }

            await _geneRepository.InsertClinicalDataAsync(clinicalDataList);
        }

        public async Task<string> ScrapeGeneData(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            var geneFileUrl = document.QuerySelector("a[href*='IlluminaHiSeq_pancan_normalized_gene']")?.GetAttribute("href");

            return geneFileUrl;
        }
    }
}
