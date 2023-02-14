"use strict";

// Class definition
var CicekSepeti_AddOrEditWizardForm = function () {
	// Base elements
	var _wizardEl;
	var _formEl;
	var _wizardObj;
	var _validations = [];

	// Private functions
	var _initValidation = function () {
		// Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
		// Step 1
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					merchantSku: {
						validators: {
							notEmpty: {
								message: 'Satıcı Stok Kodu alani zorunlu !!'
							}
						}
					},
					mainKategori: {
						validators: {
							notEmpty: {
								message: 'Boş geçilmez ERROR !!!'
                            }
                        }
                    },
					deneme: {
						validators: {
							notEmpty: {
								message: 'Boş geçilmez ERROR !!!'
							},
							emailAddress: {
								message: "siüüüüü",
								multiple: "siüüüü2",
								separator: "siüüüü3",
							}
						}
					},

				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));

		// Step 2
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					cinsiyet: {
						validators: {
							notEmpty: {
								message: 'Cinsiyet secmek zorunludur'
							}
						}
					},

				},
			}
		));

		// Step 3
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					Image1: {
						validators: {
							notEmpty: {
								message: 'Görsel 1 doldurulması zorunlu alandır.En az 1 resim eklemek zorundasınız...'
							}
						}
					},
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));

		// Step 4
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					locaddress1: {
						validators: {
							notEmpty: {
								message: 'Address is required'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));




	}

	var _initWizard = function () {
		// Initialize form wizard
		_wizardObj = new KTWizard(_wizardEl, {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		// Validation before going to next page
		_wizardObj.on('change', function (wizard) {
			if (wizard.getStep() > wizard.getNewStep()) {
				return; // Skip if stepped back
			}

			// Validate form before change wizard step
			var validator = _validations[wizard.getStep() - 1]; // get validator for currnt step

			if (validator) {
				validator.validate().then(function (status) {
					if (status == 'Valid') {
						wizard.goTo(wizard.getNewStep());

						KTUtil.scrollTop();
					} else {
						Swal.fire({
							text: "Malesef bazi hatalar tespit edildi, lutfen tekrar deneyin.",
							icon: "error",
							buttonsStyling: false,
							confirmButtonText: "Tamam anladim!",
							customClass: {
								confirmButton: "btn font-weight-bold btn-light"
							}
						}).then(function () {
							KTUtil.scrollTop();
						});
					}
				});
			}

			return false;  // Do not change wizard step, further action will be handled by he validator
		});

		// Change event
		_wizardObj.on('changed', function (wizard) {
			KTUtil.scrollTop();
		});

		// Submit event
		_wizardObj.on('submit', function (wizard) {
			Swal.fire({
				text: "Her sey yolunda! Lutfen form gonderimini onaylayin.",
				icon: "success",
				showCancelButton: true,
				buttonsStyling: false,
				confirmButtonText: "Evet, gonder!",
				cancelButtonText: "Hayir, iptal et",
				customClass: {
					confirmButton: "btn font-weight-bold btn-primary",
					cancelButton: "btn font-weight-bold btn-default"
				}
			}).then(function (result) {
				if (result.value) {
					_formEl.submit(); // Submit form
				} else if (result.dismiss === 'cancel') {
					Swal.fire({
						text: "Formunuz gonderilmedi!.",
						icon: "error",
						buttonsStyling: false,
						confirmButtonText: "Tamam anladim!",
						customClass: {
							confirmButton: "btn font-weight-bold btn-primary",
						}
					});
				}
			});
		});
	}

	return {
		// public functions
		init: function () {
			_wizardEl = KTUtil.getById('CSaddOrEditWizard');
			_formEl = KTUtil.getById('CSaddOrEditWizardForm');

			_initValidation();
			_initWizard();
		}
	};
}();

jQuery(document).ready(function () {
	CicekSepeti_AddOrEditWizardForm.init();
});
