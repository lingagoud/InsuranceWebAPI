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
    public class BuyController : ControllerBase
    {
        insuranceContext db = new insuranceContext();
        [HttpPost]
        [Route("{email}")]
        public IActionResult PostVehicle(Vehicle vehicle)
        {
            if (vehicle == null) { return BadRequest("data not recieved"); }
            try
            {

                Vehicle vehicle1 = new Vehicle();

                vehicle1 = db.Vehicles.Where(v => v.RegistrationNumber == vehicle.RegistrationNumber).FirstOrDefault();

                if (vehicle1 != null)
                {
                    Policy policy = new Policy();
                    policy = db.Policies.Where(p => p.RegistrationNumber == vehicle1.RegistrationNumber).FirstOrDefault();
                    if (policy != null)
                    {
                        Plan plan = new Plan();
                        plan = db.Plans.Where(p => p.Id == policy.PlansId).FirstOrDefault();

                        if ((DateTime.Now - policy.PurchaseDate).Days > plan.Term * 365)
                        {
                            return BadRequest("Already we have policy with this Vehicle and Insurance Duration is Completed!!\n Please Renew it through RenewInsurance in Home");
                        }
                        else
                        {
                            return BadRequest("Hurray!!! Already we have policy with this Vehicle !! \n For Extension of duration you can renew through renew insurance in Home");
                        }
                    }
                    else
                    {
                        return Ok();
                    }
                }

                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }


        }



        /// <summary>
        /// Gets Policy  based on plan and registration_number
        /// </summary>
        /// <param name="plan1"></param>
        /// <param name="reg_no"></param>
        /// <returns>Policy with 200 response</returns>
        [HttpGet]
        [Route("getplan/{reg_no}/{type}/{duration}")]

        public IActionResult GetPlan(string reg_no, string type, int duration)
        {

            // planwithoutpolicies plan1 = new planwithoutpolicies();
            Plan resPlan = new Plan();

            try
            {
                var vehicle = db.Vehicles.Where(v => v.RegistrationNumber == reg_no).FirstOrDefault();
                if (vehicle != null)
                {
                    string tp = vehicle.TypeOfVehicle;
                    resPlan = db.Plans.Where(p => (p.Term == duration) &&
                    (p.Type == type) && (p.Typeofvehicle == tp)).FirstOrDefault();

                }
                else
                {
                    return BadRequest("vehicle not found");
                }
                //    pid = from p in db.Plans where (p.Duration == plan1.Duration) && (p.Type == plan1.Type)&& p.Id == 6  select p.Id;

                if (resPlan == null) return BadRequest("plan not found");

                int days = (vehicle.PurchaseDate - DateTime.Now).Days;
                int year = Math.Abs(days) / 365;
                resPlan.Amount = amountAccordingToAge(resPlan.Amount, year);

                return Ok(resPlan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            // plan1.Amount = resPlan.Amount;
            // plan1.Duration = resPlan.Duration;
            //plan1.Id = resPlan.Id;
            //plan1.Typeofvehicle = resPlan.Typeofvehicle;
            //plan1.Type = resPlan.Type;

        }

        /// <summary>
        /// Adds policy in db based on email(user) and registration number(vehicle) 
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="email"></param>
        /// <param name="reg_no"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpolicy/{email}/{reg_no}")]
        public IActionResult PostPolicy(Plan plan, string email, string reg_no)
        {
            if (plan == null) { return BadRequest("plan not found"); }
            if (email == null) { return BadRequest("email not found"); }
            if (reg_no == null) { return BadRequest("reg no not found"); }
            try
            {

                Customer customer = new Customer();

                Policy policy = new Policy();
                customer = db.Customers.Where(e => e.Email == email).FirstOrDefault();
                if (customer == null) { return BadRequest(customer.Id+reg_no+plan.Id); }
                Console.WriteLine(customer.Id + reg_no + plan.Id);
                policy.UserId = customer.Id;
                policy.RegistrationNumber = reg_no;
                policy.PlansId = plan.Id;
                policy.PurchaseDate = DateTime.Now;
                policy.RenewAmount = plan.Amount;

                Vehicle vehicle = new Vehicle();
                vehicle = db.Vehicles.Find(reg_no);

                int days = (vehicle.PurchaseDate - DateTime.Now).Days;
                days = Math.Abs(days);
                int year = days / 365;
                policy.RenewAmount = amountAccordingToAge(plan.Amount, year);

                db.Policies.Add(policy);
                db.SaveChanges();
                return Ok();
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
