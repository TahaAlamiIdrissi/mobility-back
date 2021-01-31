using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthService.Models
{
    public class MobilityService : IMobility
    {
        private readonly AuthDbContext _context;

        public MobilityService(AuthDbContext context)
        {
            this._context = context;
        }
        public Mobility AddMobility(Mobility mobility)
        {
            this._context.Mobilties.Add(mobility);
            this._context.SaveChanges();
            return mobility;
        }

        public Mobility DeleteMobility(int id)
        {
            var mob = this._context.Mobilties.FirstOrDefault(m => m.Id == id);
            if (mob == null)
                return null;
            this._context.Mobilties.Remove(mob);
            this._context.SaveChanges();
            return mob;
        }

        public IEnumerable<Mobility> GetAllMobilities()
        {
            return this._context.Mobilties;
        }

        public IEnumerable<Mobility> GetMobilitiesByCountry(string country)
        {
            // return this._context.Mobilties.ToList().FindAll(m => m.Country == country);
            var mobilities = from m in this._context.Mobilties
                             select m;

            if (!String.IsNullOrEmpty(country))
            {
                mobilities = mobilities.Where(m => m.Country.Contains(country));
            }

            return mobilities.ToList();
        }

        public IEnumerable<Mobility> GetMobilitiesByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Mobility> GetMobilitiesByPromotion(string promo)
        {
            // return this._context.Mobilties.ToList().FindAll(m => m.Promotion == promo);
            var mobilities = from m in this._context.Mobilties
                             select m;

            if (!String.IsNullOrEmpty(promo))
            {
                mobilities = mobilities.Where(m => m.Promotion.Contains(promo));
            }

            return mobilities.ToList();
        }

        public IEnumerable<Mobility> GetMobilitiesByStudentName(string studentName)
        {
            // return this._context.Mobilties.ToList().FindAll(m => m.StudentName == studentName);
            var mobilities = from m in this._context.Mobilties
                             select m;

            if (!String.IsNullOrEmpty(studentName))
            {
                mobilities = mobilities.Where(m => m.StudentName.Contains(studentName));
            }

            return mobilities.ToList();
        }

        public IEnumerable<Mobility> GetMobilitiesMulticriteria(string promo, string country, string studentName)
        {
            var mobilities = from m in this._context.Mobilties
                             select m;

            if (!String.IsNullOrEmpty(studentName))
            {
                mobilities = mobilities.Where(m => m.StudentName.Contains(studentName));
            }
            if (!String.IsNullOrEmpty(promo))
            {
                mobilities = mobilities.Where(m => m.Promotion.Contains(promo));
            }
            if (!String.IsNullOrEmpty(country))
            {
                mobilities = mobilities.Where(m => m.Country.Contains(country));
            }

            return mobilities.ToList();
        }

        public Mobility GetMobilityById(int id)
        {
            return this._context.Mobilties.FirstOrDefault(m => m.Id == id);
        }

        public Mobility UpdateMobility(Mobility mobility)
        {
            var mob = this._context.Attach(mobility);
            mob.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
            return mobility;
        }

        public Mobility ValidateMobility(int id)
        {
            var mob = GetMobilityById(id);
            mob.State = true;
            UpdateMobility(mob);
            return mob;
        }
    }
}