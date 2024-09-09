﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_App_Scores.Models;
using BE_App_Scores.Utils;


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

        [HttpGet("VizualizareUtilizatori")]
        public ActionResult<IEnumerable<UserDto>> GetUtilizatori()
        {
            var users = _context.Users
                .Where(user => user.EmailConfirmed) // Filtrare utilizatori cu email confirmat
                .Select(user => new UserDto
                {
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("VizualizareEchipe")]
        public async Task<ActionResult<IEnumerable<Echipe>>> GetEchipe()
        {
            return await _context.Echipe.ToListAsync();
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

        // GET: api/GestionareMeciuri/scoruri
        [HttpGet("Maxscoruri")]
        public async Task<ActionResult<string>> GetMaxScor(string DenumireMeci, DateTime data)
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
                      (a, b) => new { Scor = a.s.Scor, DenumireEchipa = b.DenumireEchipa })
                .ToList();

            if (info_meci.Count < 2)
            {
                return BadRequest("Nu sunt suficiente echipe pentru a determina un câștigător.");
            }

            // Găsește scorul maxim
            var scorMaxim = info_meci.Max(e => e.Scor);

            // Găsește echipele cu scorul maxim
            var echipeCuScorMaxim = info_meci.Where(e => e.Scor == scorMaxim).Select(e => e.DenumireEchipa).ToList();

            string rezultat;

            if (echipeCuScorMaxim.Count == 1)
            {
                rezultat = $"Echipa {echipeCuScorMaxim[0]} a câștigat.";
            }
            else
            {
                rezultat = $"Există egalitate între echipele: {string.Join(", ", echipeCuScorMaxim)}.";
            }

            return Ok(rezultat);
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
        /*  [HttpPost("add/")]
          public IActionResult PostMeci(AddScore item)
          {
              string denumire_activitate;
              string denumire_meci;
              DateTime Data;

              denumire_activitate = item.denumire_activitate;
              denumire_meci = item.denumire_meci;
              Data = item.Data_Meci;


              var id_subject = _context.Activitati.Where(s => s.Titlu.Equals(denumire_activitate)).Select(s => new { IdActivitate = s.Id }).FirstOrDefault();

              return null;

          }*/

        //1.2 aici preiei doar denumire meci 

        [HttpPost("add")]
        public IActionResult PostMeci([FromBody] AddScore item)
        {
            // Verifică dacă activitatea există
            var activitate = _context.Activitati.FirstOrDefault(a => a.Titlu == item.DenumireActivitate);
            if (activitate == null)
            {
                return BadRequest("Activitatea specificată nu există.");
            }

            // Creează un nou meci
            var meci = new Meci
            {
                DenumireMeci = item.DenumireMeci,
                Data = item.DataMeci
            };
            _context.Meci.Add(meci);
            _context.SaveChanges(); // Salvează pentru a obține ID-ul meciului

            foreach (var teamScore in item.Echipe)
            {
                // Creează echipa dacă nu există
                var echipa = _context.Echipe.FirstOrDefault(e => e.DenumireEchipa == teamScore.DenumireEchipa);
                if (echipa == null)
                {
                    echipa = new Echipe { DenumireEchipa = teamScore.DenumireEchipa };
                    _context.Echipe.Add(echipa);
                    _context.SaveChanges(); // Salvează pentru a obține ID-ul echipei
                }

                // Creează scorul
                var scor = new Scoruri
                {
                    Scor = teamScore.Scor,
                    Data = item.DataMeci
                };
                _context.Scoruri.Add(scor);
                _context.SaveChanges(); // Salvează pentru a obține ID-ul scorului

                // Gestionează meciul
                var gestionareMeci = new GestionareMeci
                {
                    IdActivitate = activitate.Id,
                    IdEchipa = echipa.Id,
                    IdMeci = meci.Id,
                    IdScor = scor.Id
                };
                _context.GestionareMeciuri.Add(gestionareMeci);
            }

            _context.SaveChanges();

            return Ok(new { Message = "Meciul și scorurile au fost adăugate cu succes." });
        }


        //1.1 Creezi meciul si alegi membri din echipa
        [HttpPost("add-echipa")]
        public IActionResult CreareEchipe([FromBody] AddEchipe item)
        {
            var echipaExistenta = _context.Echipe
                               .Any(e => e.DenumireEchipa == item.DenumireEchipa);
            if (echipaExistenta)
            {
                return BadRequest("Echipa cu acest nume există deja.");
            }
            // Creează un nou meci
            var echipa = new Echipe
            {
                DenumireEchipa = item.DenumireEchipa,
            };
            _context.Echipe.Add(echipa);
            _context.SaveChanges(); // Salvează pentru a obține ID-ul meciului
            return Ok(new { Message = "Meciul și scorurile au fost adăugate cu succes." });
        }


        [HttpPost]
        [Route("Create")]
        public IActionResult Create(JoinEchipa model)
        {
            if (model == null || model.Usernames == null || string.IsNullOrEmpty(model.DenumireEchipa))
            {
                return BadRequest("Datele primite nu sunt valide.");
            }

            // Căutăm echipa după denumire
            /*  var echipa = _context.Echipe.SingleOrDefault(e => e.DenumireEchipa == model.DenumireEchipa);
              if (echipa == null)
              {
                  return NotFound();
              }*/

            var echipaExistenta = _context.Echipe
                              .Any(e => e.DenumireEchipa == model.DenumireEchipa);
            if (echipaExistenta)
            {
                return BadRequest("Echipa cu acest nume există deja.");
            }
            // Creează un nou meci
            var echipa = new Echipe
            {
                DenumireEchipa = model.DenumireEchipa,
            };
            _context.Echipe.Add(echipa);
            _context.SaveChanges();

            // Iterăm prin lista de username-uri și alocăm fiecare utilizator echipei
            foreach (var username in model.Usernames)
            {
                var user = _context.Users.SingleOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return BadRequest($"Utilizatorul {username} nu a fost găsit.");
                }

                // Verificăm dacă utilizatorul este deja alocat echipei
                var existingAllocation = _context.CreareEchipe
                    .SingleOrDefault(ce => ce.UserId == user.Id && ce.EchipeId == echipa.Id);

                if (existingAllocation != null)
                {
                    continue; // Dacă utilizatorul este deja alocat, trecem la următorul
                }

                // Creăm relația între utilizator și echipă
                var creareEchipe = new CreareEchipe
                {
                    UserId = user.Id,
                    EchipeId = echipa.Id
                };

                _context.CreareEchipe.Add(creareEchipe);
            }

            _context.SaveChanges();

            return Ok("Utilizatorii au fost alocați echipei cu succes.");
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
