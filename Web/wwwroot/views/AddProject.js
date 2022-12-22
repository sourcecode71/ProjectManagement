const app = new Vue({
    el: '#addProject',
    beforeMount() {
        sClientId = "0";
        this.loadAllcompany();
        this.LoadEmployee();
        this.LoadAllActiveProjects();
        this.ComeFromProposal();
    },
    data: {
        errors: [],
        name: '',
        budget: '',
        pweek: '',
        client:'0',
        workorderDesc: '',
        engineer: '0',
        peHours:'',
        clients: [],
        engineers: [],
        engErrors: [],
        drawing: '0',
        drawings: [],
        drawhours: '',
        drwErrors: [],
        allEmp: [],
        allEng: [],
        allDrw: [],
        allProjects: [],
        wrksBYP: [],
        companies: [],
        companyId: '0',
        proposal: '',
        proposalId: '0',
        hideNow: false,
        seen: false,
        operationType: "Submit"

    },
    methods: {
       
        SubmitProject: function () {

            this.errors = [];
            
          
            this.validateProject();

            if (this.errors.length == 0)
            {

                const config = { headers: { 'Content-Type': 'application/json' } };
                var base_url = window.location.origin;
                const Operaation = this.operationType == "Submit" ? "/api/project/create-new-project" : "/api/project/update-project";
                const clientURL = base_url + Operaation ;


                let project = {
                    id: this.id,
                    proposalId: this.proposalId,
                    name: this.name,
                    companyId: this.companyId,
                    week: this.pweek,
                    description: this.workorderDesc,
                    budget: this.budget ? this.budget:0
                }

                axios.post(clientURL, project, config)
                    .then(response => {
                        Swal.fire({
                            position: 'top-end',
                            icon: 'success',
                            title: 'Record has been added successfully!',
                            showConfirmButton: false,
                            timer: 1500
                        })

                        this.seen = this.operationType == "Submit";
                        this.operationType = "Submit";
                        this.clearAll();
                        this.LoadAllActiveProjects();
                    }).catch(errors => {
                        Swal.fire({
                            position: 'top-end',
                            title: 'Error!',
                            text: 'Something went wrong.' + errorThrown.errorMessage,
                            icon: 'error',
                            confirmButtonText: 'Ok',
                        })
                    });
            }




        },

       /************ Loading Work ****************/

        loadAllClients: function () {

            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/company/all-clients";

            axios.get(clientURL, config).then(result => {
                this.clients = result.data;
            }, error => {
                console.error(error);
            });

        },
        loadAllcompany: function () {
            const config = { headers: { "Content-Type": "application/json" } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/company/all-companies";

            axios.get(clientURL, config).then(
                (result) => {

                    console.log("  result.data ", result.data);

                    this.companies = result.data;
                },
                (error) => {
                    console.error(error);
                }
            );
        },
        LoadEmployee: function () {
            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/Employee/all-active-employee";

            axios.get(clientURL, config).then(result => {
                this.allEng = result.data.filter(p => p.role == "Engineering");
                this.allDrw = result.data.filter(p => p.role == "Drawing");
            }, error => {
                console.error(error);
            });

        },

        LoadAllActiveProjects: function () {
            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/project/load-active-projects";

            axios.get(clientURL, config).then(result => {
               // console.log(" project -- ", result);
                $("#allProject").dataTable().fnDestroy();
                this.allProjects = result.data;

                setTimeout(() => {
                    $("#allProject").DataTable({
                        scrollY: "750px",
                        scrollCollapse: true,
                        paging: false,
                        columns: [
                            { "width": "2%" },
                            { "width": "4%" },
                            { "width": "18%" },
                            { "width": "35%" },
                            { "width": "10%" },
                            { "width": "10%" },
                            { "width": "15%" }
                        ]
                    });
                }, 1000);
              
            }, error => {
                console.error(error);
            });
        },

        showWorkOrderByProject: function (prj) {


            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const wrkURL = base_url + "/api/WorkOrder/work-orders/by-project?PrId=" + prj.id;

            axios.get(wrkURL, config).then(result => {
                this.wrksBYP = result.data;
                $("#allWorkOrder").modal("show");

            }, error => {
                console.error(error);
            });



        },

        EditWorkOrderByProject: function (prj) {
            this.seen = true;
            this.name = prj.name;
            this.pweek = prj.week;
            this.workorderDesc = prj.description;
            this.companyId = prj.companyId.toLowerCase();
            this.id = prj.id;
            this.operationType = "Update";
        },

        ComeFromProposal: function () {
            var pid = this.getUrlParameter('pid');
          

            if (pid) {
                this.seen = true;

                const config = { headers: { "Content-Type": "application/json" } };
                var base_url = window.location.origin;
                const clientURL = base_url + "/api/proposal/proposal?pid=" + pid;

                axios.get(clientURL, config).then(
                    (result) => {
                        this.proposal = result.data;

                        this.proposalId = this.proposal.id;
                        this.budget = this.proposal.estimateBudget;
                        this.name = this.proposal.proposalName;
                        this.workorderDesc = this.proposal.comments;
                        this.companyId = this.proposal.companyId;

                        console.log("pid --- ", this.proposal);

                    },
                    (error) => {
                        console.error(error);
                    }
                );

            }

        },

        /************ clerical  Work ****************/

        isNumber: function (evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if ((charCode > 31 && (charCode < 48 || charCode > 57)) && charCode !== 46) {
                evt.preventDefault();;
            } else {
                return true;
            }
        },
        formatNumber: function (e) {
            let dollarUS = Intl.NumberFormat("en-US", {
                style: "currency",
                currency: "USD",
            });


            var dblBudget = this.budget.replace(",", "");
            this.budget = this.budget.charAt(0) == "$" ? dollarUS.format(dblBudget.substring(1)) : dollarUS.format(dblBudget);
        },
  
        validateProject: function () {
            if (!this.name) {
                this.errors.push("Project name is required");
            }

            if (this.companyId == "0") {
                this.errors.push("Client name is required");
            }

            if (!this.pweek) {
                this.errors.push("Project budgeted time is required");
            }

        },
        clearAll: function () {
            this.errors = [];
            this.name = '';
            this.budget = '';
            this.pweek = '';
            this.client='0';
            this.workorderDesc = '';
            this.engineers=[],
            this.engineer = '0';
            this.peHours= '';
            this.drawing = '0';
            this.drawings=[]
        },
        addNewDivVisibility() {
            this.seen = !this.seen;

            if (this.seen)
                setTimeout(() => {
                    $("#pCompany").select2({

                    }).on('change', function (e) {
                        var id = $("#pCompany option:selected").val();
                        app.companyId = id;
                        console.log(" company Id ", app.companyId);
                       
                    });

                }, 200);
        },

        getUrlParameter: function (param) {
            var results = new RegExp('[\?&]' + param + '=([^&#]*)').exec(window.location.href);
            if (results == null) {
                return null;
            }
            return decodeURI(results[1]) || 0;

        },


    },

   

});