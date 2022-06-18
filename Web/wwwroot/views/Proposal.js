const app = new Vue({
    el: '#addProject',
    beforeMount() {
        sClientId = "0";
        companyId ="0",
        this.loadAllcompany();
        this.loadAllActiveProposal();
    },
    data: {
        errors: [],
        propsalName: '',
        companyId:0,
        estimateBudget: '',
        comments: '',
        proposals: [],
        companies:[],
        hideNow: false,
        seen: false,
        selectedProposal :''

    },
    methods: {
       
        SubmitProposal: function () {
            this.errors = [];
            this.validateProposal();

            if (this.errors.length == 0)
            {

                const config = { headers: { 'Content-Type': 'application/json' } };
                var base_url = window.location.origin;
                const clientURL = base_url + "/api/proposal/create-new-proposal";

                let proposal = {
                    proposalName: this.propsalName, 
                    companyId: companyId,
                    estimateBudget: this.estimateBudget.substring(1).replace(",", ""),
                    comments: this.comments,
                    approvalStatus :0
                }
                axios.post(clientURL, proposal, config)
                    .then(response => {
                        Swal.fire({
                            position: 'top-end',
                            icon: 'success',
                            title: 'Record has been added successfully!',
                            showConfirmButton: false,
                            timer: 1500
                        });
                        this.clearAll();
                        this.loadAllActiveProposal();
                    }).catch(errorThrown => {
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

        ApprovedProposal: function (prsal)
        {
            this.selectedProposal = prsal;
            var status = prsal.approvalStatus == 0;
            if (status) {
                $("#confirmApproval").modal("show");
            }
            else {

                var Status = "Proposal already " + prsal.approvalStatusStr;
                Swal.fire({
                    position: 'top-end',
                    icon: 'info',
                    title: Status,
                    showConfirmButton: false,
                    timer: 2500
                });
            }
        },
        RejectedProposal: function (prsal) {
            this.selectedProposal = prsal;
            var status = prsal.approvalStatus == 0;
            if (status) {
                $("#confirmReject").modal("show");
            }
            else {

                var Status = "Proposal already " + prsal.approvalStatusStr;

                Swal.fire({
                    position: 'top-end',
                    icon: 'info',
                    title: Status,
                    showConfirmButton: false,
                    timer: 2500
                });
            }
        },

        ConfirmApprovedProposal: function () {
            var id = this.selectedProposal.id;
            window.location.href = '/admin/AddProject?pid=' + id;
        },


        ConfirmRejectProposal: function () {
            const config = { headers: { "Content-Type": "application/json" } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/proposal/change-proposal-status?pId=" + this.selectedProposal.id +"&appStatus="+4;

            axios.put(clientURL, config).then(
                (result) => {
                    var Status = "";
                    if (result.data) {
                        $("#confirmReject").modal("hide");
                        this.loadAllActiveProposal();
                        Status = "Proposal has been rejected.";
                    } 

                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: Status,
                        showConfirmButton: false,
                        timer: 2500
                    });
                },
                (errorThrown) => {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'Error',
                        title:'Error',
                        text: 'Something went wrong.' + errorThrown.errorMessage,
                        showConfirmButton: false,
                        timer: 2500
                    });
                }
            );
        },

       /************ Loading Company ****************/
      
        loadAllcompany: function () {
            const config = { headers: { "Content-Type": "application/json" } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/company/all-companies";

            axios.get(clientURL, config).then(
                (result) => {
                    this.companies = result.data;
                    console.log(" projects ", this.companies);

                },
                (error) => {
                    console.error(error);
                }
            );
        },

        loadAllActiveProposal: function() {
            const config = { headers: { "Content-Type": "application/json" } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/proposal/all-active-proposal";

            axios.get(clientURL, config).then(
                (result) => {
                    this.proposals = result.data;
                    $("#allProposal").dataTable().fnDestroy();

                    setTimeout(() => {
                        $("#allProposal").DataTable({
                            scrollY: "500px",
                            scrollCollapse: true,
                            paging: false,
                            columns: [
                                { "width": "3%" },
                                { "width": "9%" },
                                { "width": "12%" },
                                { "width": "24%" },
                                { "width": "10%" },
                                { "width": "8%" },
                                { "width": "12%" },
                                { "width": "22%" }
                            ]
                        });

                    }, 1000);


                },
                (error) => {
                    console.error(error);
                }
            );
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


            var dblBudget = this.estimateBudget.replace(",", "");
            this.estimateBudget = this.estimateBudget.charAt(0) == "$" ? dollarUS.format(dblBudget.substring(1)) : dollarUS.format(dblBudget);
        },
  
        validateProposal: function () {
            if (!this.propsalName) {
                this.errors.push("Proposal name is required");
            }

            if (companyId == "0" || !companyId) {
                this.errors.push("Company name is required");
            }

            if (!this.estimateBudget) {
                this.errors.push("Proposal budget is required");
            }

        },
        clearAll: function () {
            this.errors = [];
            this.propsalName = '';
            this.estimateBudget = 0;
            this.comments = '';
            this.companyId = '0';
            this.drawings=[]
        },
        addNewDivVisibility() {
            this.seen = !this.seen;

            if (this.seen)
                setTimeout(() => {
                    $("#pCompany").select2({

                    }).on('change', function (e) {
                        var id = $("#pCompany option:selected").val();
                        companyId = id;
                        app.companyId = id;                       
                    });

                }, 200);
        },
    },

   

});