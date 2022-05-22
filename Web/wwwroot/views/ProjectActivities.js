﻿
const app = new Vue({
    el: '#projectAct',
    beforeMount() {
        this.LoadActiveWrkBudget();

    },
    data: {
        errors: [],
        appStatus: 1,
        isApproved: true,
        appBudget: 0,
        comment: null,
        bLabel:"Approved Budget",
        seen: false,
        appComment:null,
        wrkBudgets: [],
        wrkBudgetsHis: [],
        selectedWrk: null,
        companies: [],
        wrkStatus: [],
        wrkState:"0",
        budgetNo: '',
        wrkName: '',
        submitDate: '',
        wrkId: '',
        wrkStatus: '',
        wrkComments: '',
        originalBudget:0

    },
    methods: {
        approvalSubmit: function () {
            this.errors = [];
            const config = { headers: { "Content-Type": "application/json" } };
            var base_url = window.location.origin;
            const approvalURL = base_url + "/api/project/budegt-approval/status";

            var wrkData = {
                approvedBudget: this.appBudget.substring(1).replace(",", ""),
                status: this.appStatus,
                comments: this.appComment,
                workOrderId: this.wrkId
            }

            if (this.IsValidForm()) {
                axios
                    .post(approvalURL, wrkData, config)
                    .then((response) => {
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Record has been added successfully!",
                            showConfirmButton: false,
                            timer: 1500,
                        });

                        this.clearAll();
                        this.LoadActiveWrkBudget();
                    })
                    .catch((errors) => {
                        Swal.fire({
                            position: "top-end",
                            title: "Error!",
                            text: "Something went wrong." + errorThrown.errorMessage,
                            icon: "error",
                            confirmButtonText: "Ok",
                        });
                    });
            }
        },
        
        approvalPop: function (wrk) {

            this.selectedWrk = wrk;
            if (wrk.approvalStatus == 0 || wrk.approvalStatus == 3) {
                this.appBudget = this.formatCurrenct(wrk.budget);
                this.wrkName = wrk.consecutiveWork;
                this.budgetNo = wrk.budegtNo;
                this.submitDate = wrk.budgetSubmitDateStr;
                this.wrkId = wrk.workOrderId;

                this.LoadOriginalBudget(wrk.workOrderId);

                $("#exampleModal").modal("show");

            } else {
                var msg = wrk.approvalStatus  == 1 ? "Already the budget is approved" : "Already the budget is refused";

                Swal.fire({
                    position: 'top-end',
                    title: 'Info!',
                    text: msg,
                    icon: 'info',
                    showConfirmButton: false,
                    timer: 2000
                });
            }
        },

        LoadOriginalBudget: function (wrkId) {

            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/WorkOrder/work-order-budget?wrkId=" + wrkId;

            axios.get(clientURL, config).then(result => {
                this.originalBudget = this.formatCurrenct(result.data.originalBudget);
            }, error => {
                console.error(error);
            });

        },

        LoadBudgetHistory: function (wrk) {
            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/WorkOrder/work-order-budget?wrkId=" + wrk.workOrderId;

            axios.get(clientURL, config).then(result => {
                this.wrkBudgetsHis = result.data.bHistory;
                $("#budgetHistoryModal").modal("show");
            }, error => {
                console.error(error);
            });

        },

        statusChangeSubmit: function () {
            this.errors = [];
            const config = { headers: { "Content-Type": "application/json" } };
            var base_url = window.location.origin;
            const approvalURL = base_url + "/api/workOrder/status-change";

            var wrkStatus = {
                workOrderId: this.workOrderId,
                status: this.wrkState
            }


            if (this.isValidStatusFrom()) {
                axios
                    .put(approvalURL, wrkStatus, config)
                    .then((response) => {
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Record has been added successfully!",
                            showConfirmButton: false,
                            timer: 1500,
                        });

                        this.clearAll();
                        this.LoadActiveWrkBudget();
                        this.LoadActiveStatusBUdget();
                        $("#projectstatusModal").modal("hide");
                    })
                    .catch((errors) => {
                        Swal.fire({
                            position: "top-end",
                            title: "Error!",
                            text: "Something went wrong." + errorThrown.errorMessage,
                            icon: "error",
                            confirmButtonText: "Ok",
                        });
                    });
            }



        },
        statusChangePop: function (wrk) {
            this.errors = [];
            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/project/project-status";
            this.workOrderId = wrk.workOrderId;

            axios.get(clientURL, config).then(result => {
              
                $("#projectstatusModal").modal("show");

                this.wrkStatus = result.data;

             

            }, error => {
                console.error(error);
            });

        },

        LoadActiveWrkBudget: function () {
            var PmName = "NA";
            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/project/project-activities/budget-load?PmName=" + PmName;

            setTimeout(() => {
                $("#allwrkBudget").dataTable().fnDestroy();

            }, 100);

            axios.get(clientURL, config).then(result => {
                this.wrkBudgets = result.data;

                setTimeout(() => {
                    $('#allwrkBudget').DataTable({
                        "scrollY": "500px",
                        "scrollCollapse": true,
                        "paging": false,
                        "columns": [
                            { "width": "2%" },
                            { "width": "10%" },
                            { "width": "8%" },
                            { "width": "20%" },
                            { "width": "6%" },
                            { "width": "6%" },
                            { "width": "8%" },
                            { "width": "8%" },
                            { "width": "6%" },
                            { "width": "10%" },
                        ]
                    });

                }, 500);

            }, error => {
                console.error(error);
            });

        },

        LoadActiveStatusBUdget: function () {

            var PmName = "NA";
            const config = { headers: { 'Content-Type': 'application/json' } };
            var base_url = window.location.origin;
            const clientURL = base_url + "/api/project/project-activities/budget-load?PmName=" + PmName;

            axios.get(clientURL, config).then(result => {
                this.wrkBudgets = result.data;

                $("#allwrkBudgetStatus").dataTable().fnDestroy();


                setTimeout(() => {
                    $('#allwrkBudgetStatus').DataTable({
                        "scrollY": "500px",
                        "scrollCollapse": true,
                        "paging": false,
                        "columns": [
                            { "width": "2%" },
                            { "width": "8%" },
                            { "width": "5%" },
                            { "width": "20%" },
                            { "width": "8%" },
                            { "width": "6%" },
                            { "width": "6%" },
                            { "width": "5%" },
                            { "width": "5%" },
                            { "width": "6%" },
                            { "width": "8%" },
                        ]
                    });
                }, 500);



            }, error => {
                console.error(error);
            });

        },

        onChangeStatus: function (status) {
            console.log(" status ", status);
            this.bLabel = status == 1 ? " Approved Budget" : "Change Budget";
            if (status == 2) {
                this.isApproved = false;
            } else {
                this.isApproved = true;
            }
            
        },

        onTabBudgetClick: function () {
            this.LoadActiveWrkBudget();
        },

        onTabStatueClick: function () {
            this.LoadActiveStatusBUdget();
        },
 
        clearAll: function () {
            this.appStatus = "1";
            this.appComment = "";
            this.appBudget = "";
            this.wrkStatus = "0";
            this.wrkComments = "";
        },
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

            console.log(" char at ", this.appBudget.charAt(0))

            var dblBudget = this.appBudget.replace(",", "");
            this.appBudget = this.appBudget.charAt(0) == "$" ? dollarUS.format(dblBudget.substring(1)) : dollarUS.format(dblBudget);
        },
        formatCurrenct: function (crnMoney) {
            let dollarUS = Intl.NumberFormat("en-US", {
                style: "currency",
                currency: "USD",
            });
            return dollarUS.format(crnMoney);
        },
        IsValidForm: function () {
            this.errors = [];


            if (!this.appBudget && (this.appStatus == 3 || this.appStatus == 1) ) {
                this.errors("The approved budget is needed.");
            }

            console.log(" this.appBudget ", this.errors);

            if (!this.errors.length) {
                return true;
            } else {
                return false;
            }

        },
        isValidStatusFrom: function () {
            this.errors = [];

            if (this.wrkState == "0") {
                this.errors.push("Please select the work order status")
            }

            if (!this.errors.length) {
                return true;
            } else {
                return false;
            }
        },


    }
})




