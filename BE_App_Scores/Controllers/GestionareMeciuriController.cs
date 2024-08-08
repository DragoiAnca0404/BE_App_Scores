using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_App_Scores.Models;

namespace BE_App_Scores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionareMeciuriController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GestionareMeciuriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GestionareMeciuri
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meci>>> GetMeci()
        {
            return await _context.Meci.ToListAsync();
        }

        // GET: api/GestionareMeciuri
        [HttpGet("VizualizareActivitati")]
        public async Task<ActionResult<IEnumerable<Activitate>>> GetActivitati()
        {
            return await _context.Activitati.ToListAsync();
        }

        // GET: api/GestionareMeciuri/5
        [HttpGet("activitate")]
        public async Task<ActionResult<Activitate>> GetActivitateMeciuri(string DenumireActivitate)
        {

            var id_activitate = _context.Activitati
               .Where(m => m.Titlu == DenumireActivitate)
               .Select(y => new { id_activitate = y.Id }).ToList();

            var info_meci = _context.Activitati.Where(m => m.Id.Equals(id_activitate.First().id_activitate))
          .Join(_context.GestionareMeciuri,
                u => u.Id,
                s => s.IdActivitate,
                (u, s) => new
                { s, u })
          .Join(_context.Meci,
          a => a.s.IdMeci,
          b => b.Id,
          (a, b) => new
          { a, b })
          .GroupBy(m => new { m.b.DenumireMeci, m.b.Data }) // Grupați după denumirea meciului și dată
          .Select(y => new { denumire_meciuri = y.Key.DenumireMeci, data = y.Key.Data }).ToList();

            return Ok(info_meci);
        }
        // GET: api/GestionareMeciuri/scoruri
        [HttpGet("scoruri")]
        public async Task<ActionResult<IEnumerable<object>>> GetScoruriInFunctieDeMeci(string DenumireMeci, DateTime data)
        {
            var id_meci = _context.Meci
                .Where(m => m.DenumireMeci == DenumireMeci && m.Data == data)
                .Select(m => m.Id)
                .FirstOrDefault();

            if (id_meci == 0)
            {
                return NotFound("Meciul nu a fost găsit.");
            }

            var info_meci = _context.GestionareMeciuri
                .Where(g => g.IdMeci == id_meci)
                .Join(_context.Scoruri,
                      g => g.IdScor,
                      s => s.Id,
                      (g, s) => new { g, s })
             .Join(_context.Echipe,
          a => a.g.IdEchipa,
          b => b.Id,
          (a, b) => new { a, b })
          .Select(y => new { Scor = y.a.g.Scoruri.Scor, denumire_echipa = y.b.DenumireEchipa}).ToList();

            return Ok(info_meci);
        }



        // GET: api/GestionareMeciuri/5
        [HttpGet("{DenumireMeci}")]
        public async Task<ActionResult<Meci>> GetScoruriMeci(string DenumireMeci)
        {
            var id_meci =  _context.Meci
                .Where(m => m.DenumireMeci == DenumireMeci)
                .Select(y => new { id_meci = y.Id }).ToList();



            var gestionari = _context.GestionareMeciuri.ToList();
            var meciuri = _context.Meci.Where(m => m.Id.Equals(id_meci.First().id_meci)).ToList();

            // Verifică dacă sunt rezultate în fiecare listă
            Console.WriteLine($"Număr meciuri: {meciuri.Count}");
            Console.WriteLine($"Număr gestionari: {gestionari.Count}");


            var info_meci = _context.Meci.Where(m=>m.Id.Equals(id_meci.First().id_meci))
          .Join(_context.GestionareMeciuri,
                u => u.Id,
                s => s.IdMeci,
                (u, s) => new
                { s, u })
          .Join(_context.Activitati,
                x => x.s.IdActivitate,
                y => y.Id,
                (x, y) => new { x, y })
          .Join(_context.Echipe,
          a=> a.x.s.IdEchipa,
          b=> b.Id,
          (a, b) => new { a, b })
          .Join(_context.Scoruri,
          h => h.a.x.s.IdScor,
          g => g.Id,
          (h, g) => new { h, g })
          .Select(y => new { Scorul = y.g.Scor, data = y.g.Data, Activitate= y.h.a.y.Titlu, Echipa = y.h.a.x.s.Echipa.DenumireEchipa }).ToList();


            if (id_meci == null)
            {
                return NotFound();
            }

            return Ok(info_meci);
        }

        // PUT: api/GestionareMeciuri/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeci(int id, Meci meci)
        {
            if (id != meci.Id)
            {
                return BadRequest();
            }

            _context.Entry(meci).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeciExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GestionareMeciuri
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Meci>> PostMeci(Meci meci)
        {
            _context.Meci.Add(meci);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeci", new { id = meci.Id }, meci);
        }

        // DELETE: api/GestionareMeciuri/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeci(int id)
        {
            var meci = await _context.Meci.FindAsync(id);
            if (meci == null)
            {
                return NotFound();
            }

            _context.Meci.Remove(meci);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MeciExists(int id)
        {
            return _context.Meci.Any(e => e.Id == id);
        }
    }
}
