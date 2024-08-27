using System;
using System.Collections.Generic;
using Zermos_Web.Models.SomtodayRoosterModel;
using Zermos_Web.Models.zermelo;
using Action = Zermos_Web.Models.zermelo.Action;

namespace Zermos_Web.Utilities;

public static class SomtodayUtils
{
    public static ZermeloRoosterModel TransformToZermeloRoosterModel(this SomtodayRoosterModel somtodayItems)
    {
        if (somtodayItems == null || somtodayItems.items == null || somtodayItems.items.Count == 0)
        {
            return new ZermeloRoosterModel
                {response = new Response {data = new List<Items> {new() {appointments = new List<Appointment>()}}}};
        }


        var zermeloModel = new ZermeloRoosterModel
        {
            response = new Response
            {
                data = new List<Items>
                {
                    new()
                    {
                        MondayOfAppointmentsWeek = somtodayItems.MondayOfAppointmentsWeek,
                        appointments = new List<Appointment>()
                    }
                }
            },
            timeStamps = somtodayItems.timeStamps,
            roosterOrigin = "somtoday",
        };

        foreach (var item in somtodayItems.items)
        {
            //begin en einddatumtijd zijn allebij DateTimes vanuit nederlandse tijdzone, zet deze om naar unix timestamps (UTC)
            int start = item.beginDatumTijd.DutchDateTimeToUnixTimestamp();
            int end = item.eindDatumTijd.DutchDateTimeToUnixTimestamp();
            
            var appointment = new Appointment
            {
                start = start,
                end = end,
                cancelled = false, // (a SOMtoday item can never be cancelled, a cancelled appointment would not be in the list)
                appointmentType = item.afspraakItemType,
                subjects = new List<string> {item.vak?.naam ?? "N/A"},
                locations = new List<string> {item.locatie},
                teachers = item.docentNamen,
                actions = new List<Action>()
            };

            zermeloModel.response.data[0].appointments.Add(appointment);
        }

        return zermeloModel;
    }
}