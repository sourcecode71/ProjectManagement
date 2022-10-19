const app = new Vue({
    el: '#Authentication',
    beforeMount() {
        sClientId = "0";
        companyId ="0"
    },
    data: {
        errors: [],
        email: '',
        password: '',
        seen: false
    },
    methods: {
        SubmitForLogin: function () {
            this.errors = [];
            this.validateProposal();
            if (this.errors.length == 0)
            {
                const config = { headers: { 'Content-Type': 'application/json' } };
                var base_url = window.location.origin;
                const clientURL = base_url + "/api/Employee/login";

                let proposal = {
                    email: this.email,
                    password: this.password
                }
                axios.post(clientURL, proposal, config)
                    .then(response => {
                        if (response.status == 200) {
                            var vData = response.data;

                            console.log("response ", vData.isSuccess);

                            console.log("response ", vData);


                            if (vData.isSuccess) {
                                window.location.href = "/Admin/Index";
                            }
                            else {
                                this.seen = true;
                            }

                            this.clearAll();
                        } else {

                        }

                    }).catch(errorThrown => {
                        this.seen = true;
                    });
            }
        },

        /************ clerical  Work ****************/
        validateProposal: function () {
            if (!this.email) {
                this.errors.push("Email name is required");
            }

            if (!this.password) {
                this.errors.push("Password is required");
            }
        },
        clearAll: function () {
            this.email = [];
            this.password = '';
        }
    },

   

});