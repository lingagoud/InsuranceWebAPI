using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsuranceWebAPI.models;

namespace InsuranceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RenewController : ControllerBase
    {
        insuranceContext db = new insuranceContext();

        //--GetPlanForReview
        [HttpGet]
        [Route("getplanrenew/{policy_id}/{type}/{duration}")]
        public IActionResult GetPlanRenew(int policy_id, string type, int duration)
        {

            try
            {
                Policy policy = new Policy();
                Vehicle vehicle = new Vehicle();
                Plan plan = new Plan();

                policy = db.Policies.Find(policy_id);
                vehicle = db.Vehicles.Find(policy.RegistrationNumber);

                if (vehicle != null)
                {
                    string tp = vehicle.TypeOfVehicle;
                    plan = db.Plans.Where(p => (p.Term == duration) &&
                    (p.Type == type) && (p.Typeofvehicle == tp)).FirstOrDefault();
                }
                else
                {
                    return BadRequest("vehicle not found");
                }
                //    pid = from p in db.Plans where (p.Duration == plan1.Duration) && (p.Type == plan1.Type) && p.Id == 6  select p.Id;

                if (plan == null) return BadRequest("plan not found");

                //calculating age of vehicle
                int days = (vehicle.PurchaseDate - DateTime.Now).Days;
                days = Math.Abs(days);
                int year = days / 365;
                plan.Amount = amountAccordingToAge(plan.Amount, year);

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }


        }


        //---RenewPolicy--
        [HttpPut]
        [Route("renewpolicy/{policy_id}")]
        public IActionResult putPolicy(int policy_id, Plan plan)
        {

            try
            {
                Policy policy = new Policy();
                policy = db.Policies.Find(policy_id);

                policy.PlansId = plan.Id;
                policy.PurchaseDate = DateTime.Now;
                // policy.RenewAmount += plan.Amount;

                Vehicle vehicle = new Vehicle();
                vehicle = db.Vehicles.Find(policy.RegistrationNumber);

                int days = (vehicle.PurchaseDate - DateTime.Now).Days;
                days = Math.Abs(days);
                int year = days / 365;
                policy.RenewAmount += amountAccordingToAge(plan.Amount, year);


                db.SaveChanges();

                return Ok(policy);
            }
            catch (Exception Ex)
            {

                return BadRequest(Ex.InnerException.Message);
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
