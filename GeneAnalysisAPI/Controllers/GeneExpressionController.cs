using GeneAnalysisAPI.Models;
using GeneAnalysisAPI.Repositories;
using GeneAnalysisAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeneAnalysisAPI.Controllers
{
    [Route("api/gene-expression")]
    [ApiController]
    public class GeneExpressionController : ControllerBase
    {
        private readonly IGeneRepository _geneRepository;
        private readonly ScraperService _scraperService;
        public GeneExpressionController(IGeneRepository geneRepository, ScraperService scraperService)
        {
            _geneRepository = geneRepository;
            _scraperService = scraperService;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetGeneDataWithClinicalInfo(string patientId)
        {
            var result = await _geneRepository.GetGeneDataWithClinicalInfoAsync(patientId);
            if (result == null || result.Count == 0)
                return NotFound("No data found for the patient ID.");

            return Ok(result);
        }

        [HttpPost("process-clinical-tsv")]
        public async Task<IActionResult> ProcessClinicalTsv([FromBody] ClinicalTsvProcessingRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FilePath))
                return BadRequest("Invalid request. Please provide a valid file path.");

            try
            {
                await _scraperService.ProcessClinicalTsvFileAsync(request.FilePath);
                return Ok(new { message = "Clinical TSV file processed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing clinical file", error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<ActionResult<List<GeneData>>> GetAllGenes()
        {
            var genes = await _geneRepository.GetAllGenesAsync();
            return Ok(genes);
        }

        [HttpGet("{patientId}")]
        public async Task<ActionResult<List<GeneData>>> GetGenesByPatient(string patientId)
        {
            var genes = await _geneRepository.GetGenesByPatientIdAsync(patientId);
            if (genes == null) return NotFound();
            return Ok(genes);
        }

        [HttpPost]
        public async Task<ActionResult> InsertGene([FromBody] GeneData gene)
        {
            await _geneRepository.InsertGeneAsync(gene);
            return CreatedAtAction(nameof(GetGenesByPatient), new { patientId = gene.PatientId }, gene);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGene(string id, [FromBody] GeneData updatedGene)
        {
            var existingGene = await _geneRepository.GetGeneByIdAsync(id);
            if (existingGene == null) return NotFound();

            await _geneRepository.UpdateGeneAsync(id, updatedGene);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGene(string id)
        {
            var existingGene = await _geneRepository.GetGeneByIdAsync(id);
            if (existingGene == null) return NotFound();

            await _geneRepository.DeleteGeneAsync(id);
            return NoContent();
        }
        public class ClinicalTsvProcessingRequest
        {
            public string FilePath { get; set; }
        }
        public class TsvProcessingRequest
        {
            public string FilePath { get; set; }
            public string CancerCohort { get; set; }
        }
    }
    }
