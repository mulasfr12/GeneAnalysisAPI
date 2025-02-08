using GeneAnalysisAPI.Models;
using GeneAnalysisAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GeneAnalysisAPI.Controllers
{
    [Route("api/gene-expression")]
    [ApiController]
    public class GeneExpressionController : ControllerBase
    {
        private readonly IGeneRepository _geneRepository;

        public GeneExpressionController(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository;
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
    }
    }
