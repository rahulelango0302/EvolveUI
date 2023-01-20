
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EvolveWebApp.Model
{
    public class FormModel
    {
        
        public string Type { get; set; }

        /*       
                [DefaultValue("false")]*/
        [Required(ErrorMessage = "This field is required.")]
        public string SecurityGroup { get; set; }

        public string EmpId { get; set; }

        public string Name { get; set; }
        public string ObjectId { get; set; }
        public string Dept { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string  TicketNumber{ get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
       
        public string TrainingName { get; set; }
       

        public string TrainerName { get; set; }



        public void Save(FormModel formModel)
        {
            FormDB formDB= new FormDB();
            formDB.Save(formModel);

        }
    }

}
