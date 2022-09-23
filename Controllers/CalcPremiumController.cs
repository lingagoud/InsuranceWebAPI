using InsuranceWebAPI.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalcPremiumController : ControllerBase
    {

        insuranceContext db = new insuranceContext();


        /// <summary>
        /// Calculation Estimate Vehicle
        /// </summary>
        /// <param name="typeofvehicle"></param>
        /// <param name="type"></param>
        /// <param name="duration"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("{typeofvehicle}/{type}/{duration}/{age}")]

        public IActionResult GetPlanCalc(string typeofvehicle, string type, int duration, int age)
        {

            try
            {

                Plan plan = new Plan();
                plan = db.Plans.Where(p => (p.Term == duration) &&
                 (p.Type == type) && (p.Typeofvehicle == typeofvehicle)).FirstOrDefault();

                if (plan == null) return BadRequest("plan not found");


                plan.Amount = amountAccordingToAge(plan.Amount, age);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        private decimal amountAccordingToAge(decimal planAmount, int Age)
        {


            if (Age < 3)
            {
                return planAmount;
            }
            else if (Age > 3 && Age < 5)
            {
                planAmount += (planAmount * 5) / 100;
            }
            else if (Age > 5 && Age < 10)
            {
                planAmount += (planAmount * 10) / 100;
            }
            else if (Age > 10 && Age < 20)
            {
                planAmount += (planAmount * 20) / 100;
            }
            else
            {
                planAmount += (planAmount * 30) / 100;

            }

            return planAmount;

        }

    }
}
