using System;
using System.Collections.Generic;

namespace AuthService.Models
{
    public interface IMobility
    {
        Mobility AddMobility(Mobility mobility);
        Mobility DeleteMobility(int id);
        Mobility UpdateMobility(Mobility mobility);
        Mobility GetMobilityById(int id);
        Mobility ValidateMobility(int id);
        IEnumerable<Mobility> GetAllMobilities();
        IEnumerable<Mobility> GetMobilitiesByStudentName(string studentName);
        IEnumerable<Mobility> GetMobilitiesByCountry(string country);
        IEnumerable<Mobility> GetMobilitiesByPromotion(string promo);
        IEnumerable<Mobility> GetMobilitiesMulticriteria(string? promo,string? country,string? studentName);

        IEnumerable<Mobility> GetMobilitiesByDate(DateTime date);
    }
}