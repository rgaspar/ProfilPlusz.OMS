const App = {
    setup() {

        const state = Vue.reactive({
            mainData: [],
            secondaryData: [],
            deleteMode: false,
            mainTitle: null,
            manageContactTitle: 'Kapcsolattartó kezelése',
            isSubmitting: false,

            id: '',
            number: '',
            name: '',
            description: '',
            contactPersonName: '',
            phoneNumber: '',
            faxNumber: '',
            emailAddress: '',
            emailAddressOrderConfirmation: '',
            emailAddressInvoice: '',
            emailAddressPurchaseOrder: '',
            website: '',
            taxNumber: '',
            euTaxNumber: '',
            bankAccountNumber: '',
            invoiceType: null,
            paymentMethod: null,
            paymentDeadlineDays: null,
            currency: null,
            street: '',
            city: '',
            zipCode: '',
            country: '',

            customerGroupId: null,
            customerCategoryId: null,
            customerGroupListLookupData: [],
            customerCategoryListLookupData: [],

            errors: {
                name: '',
                emailAddress: '',
                customerGroupId: '',
                customerCategoryId: ''
            }
        });

        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);
        const manageContactModalRef = Vue.ref(null);
        const secondaryGridRef = Vue.ref(null);
        const customerGroupIdRef = Vue.ref(null);
        const customerCategoryIdRef = Vue.ref(null);
        const invoiceTypeRef = Vue.ref(null);
        const paymentMethodRef = Vue.ref(null);
        const paymentDeadlineDaysRef = Vue.ref(null);
        const currencyRef = Vue.ref(null);

        const invoiceTypeLookup = [
            { value: 1, text: 'Elektronikus' },
            { value: 2, text: 'Papír' }
        ];
        const paymentMethodLookup = [
            { value: 1, text: 'Átutalás' },
            { value: 2, text: 'Készpénz' }
        ];
        const paymentDeadlineDaysLookup = [
            { value: 8, text: '8 nap' },
            { value: 15, text: '15 nap' },
            { value: 30, text: '30 nap' }
        ];
        const currencyLookup = [
            { value: 1, text: 'HUF' },
            { value: 2, text: 'EUR' },
            { value: 3, text: 'USD' }
        ];

        const services = {
            getMainData: () => AxiosManager.get('/Customer/GetCustomerList', {}),

            createMainData: payload =>
                AxiosManager.post('/Customer/CreateCustomer', payload),

            updateMainData: payload =>
                AxiosManager.post('/Customer/UpdateCustomer', payload),

            deleteMainData: (id, deletedById) =>
                AxiosManager.post('/Customer/DeleteCustomer', { id, deletedById }),

            getCustomerGroupListLookupData: () =>
                AxiosManager.get('/CustomerGroup/GetCustomerGroupList', {}),

            getCustomerCategoryListLookupData: () =>
                AxiosManager.get('/CustomerCategory/GetCustomerCategoryList', {}),

            getSecondaryData: customerId =>
                AxiosManager.get('/CustomerContact/GetCustomerContactByCustomerIdList?customerId=' + customerId, {}),

            createSecondaryData: (name, jobTitle, phoneNumber, emailAddress, description, customerId, createdById) =>
                AxiosManager.post('/CustomerContact/CreateCustomerContact', {
                    name, jobTitle, phoneNumber, emailAddress, description, customerId, createdById
                }),

            updateSecondaryData: (id, name, jobTitle, phoneNumber, emailAddress, description, customerId, updatedById) =>
                AxiosManager.post('/CustomerContact/UpdateCustomerContact', {
                    id, name, jobTitle, phoneNumber, emailAddress, description, customerId, updatedById
                }),

            deleteSecondaryData: (id, deletedById) =>
                AxiosManager.post('/CustomerContact/DeleteCustomerContact', { id, deletedById }),
        };

        const methods = {
            populateMainData: async () => {
                const response = await services.getMainData();
                state.mainData = (response?.data?.content?.data ?? []).map(item => ({
                    ...item,
                    createdAtUtc: new Date(item.createdAtUtc)
                }));
            },

            populateCustomerGroupListLookupData: async () => {
                const response = await services.getCustomerGroupListLookupData();
                state.customerGroupListLookupData = response?.data?.content?.data ?? [];
            },

            populateCustomerCategoryListLookupData: async () => {
                const response = await services.getCustomerCategoryListLookupData();
                state.customerCategoryListLookupData = response?.data?.content?.data ?? [];
            },

            populateSecondaryData: async (customerId) => {
                const response = await services.getSecondaryData(customerId);
                state.secondaryData = (response?.data?.content?.data ?? []).map(item => ({
                    ...item,
                    createdAtUtc: new Date(item.createdAtUtc)
                }));
            },
        };

        const resetFormState = () => {
            Object.assign(state, {
                id: '', number: '', name: '', description: '',
                contactPersonName: '', phoneNumber: '', faxNumber: '',
                emailAddress: '', emailAddressOrderConfirmation: '',
                emailAddressInvoice: '', emailAddressPurchaseOrder: '',
                website: '', taxNumber: '', euTaxNumber: '', bankAccountNumber: '',
                invoiceType: null, paymentMethod: null,
                paymentDeadlineDays: null, currency: null,
                street: '', city: '', zipCode: '', country: '',
                customerGroupId: null, customerCategoryId: null,
                errors: { name: '', emailAddress: '', customerGroupId: '', customerCategoryId: '' }
            });
            if (customerGroupDropdown) customerGroupDropdown.value = null;
            if (customerCategoryDropdown) customerCategoryDropdown.value = null;
            if (invoiceTypeDropdown) invoiceTypeDropdown.value = null;
            if (paymentMethodDropdown) paymentMethodDropdown.value = null;
            if (paymentDeadlineDaysDropdown) paymentDeadlineDaysDropdown.value = null;
            if (currencyDropdown) currencyDropdown.value = null;
        };

        const populateFormFromRecord = (r) => {
            Object.assign(state, {
                id: r.id ?? '',
                number: r.number ?? '',
                name: r.name ?? '',
                description: r.description ?? '',
                contactPersonName: r.contactPersonName ?? '',
                phoneNumber: r.phoneNumber ?? '',
                faxNumber: r.faxNumber ?? '',
                emailAddress: r.emailAddress ?? '',
                emailAddressOrderConfirmation: r.emailAddressOrderConfirmation ?? '',
                emailAddressInvoice: r.emailAddressInvoice ?? '',
                emailAddressPurchaseOrder: r.emailAddressPurchaseOrder ?? '',
                website: r.website ?? '',
                taxNumber: r.taxNumber ?? '',
                euTaxNumber: r.euTaxNumber ?? '',
                bankAccountNumber: r.bankAccountNumber ?? '',
                invoiceType: r.invoiceType ?? null,
                paymentMethod: r.paymentMethod ?? null,
                paymentDeadlineDays: r.paymentDeadlineDays ?? null,
                currency: r.currency ?? null,
                street: r.street ?? '',
                city: r.city ?? '',
                zipCode: r.zipCode ?? '',
                country: r.country ?? '',
                customerGroupId: r.customerGroupId ?? null,
                customerCategoryId: r.customerCategoryId ?? null,
            });
            if (customerGroupDropdown) customerGroupDropdown.value = state.customerGroupId;
            if (customerCategoryDropdown) customerCategoryDropdown.value = state.customerCategoryId;
            if (invoiceTypeDropdown) invoiceTypeDropdown.value = state.invoiceType;
            if (paymentMethodDropdown) paymentMethodDropdown.value = state.paymentMethod;
            if (paymentDeadlineDaysDropdown) paymentDeadlineDaysDropdown.value = state.paymentDeadlineDays;
            if (currencyDropdown) currencyDropdown.value = state.currency;
        };

        let customerGroupDropdown = null;
        let customerCategoryDropdown = null;
        let invoiceTypeDropdown = null;
        let paymentMethodDropdown = null;
        let paymentDeadlineDaysDropdown = null;
        let currencyDropdown = null;

        const handler = {
            handleSubmit: async function () {
                try {
                    state.isSubmitting = true;
                    await new Promise(resolve => setTimeout(resolve, 200));

                    state.errors = { name: '', emailAddress: '', customerGroupId: '', customerCategoryId: '' };
                    let isValid = true;

                    if (!state.name) { state.errors.name = 'Név megadása kötelező.'; isValid = false; }
                    if (!state.emailAddress) { state.errors.emailAddress = 'E-mail cím megadása kötelező.'; isValid = false; }
                    if (!state.customerGroupId) { state.errors.customerGroupId = 'Ügyfélcsoport megadása kötelező.'; isValid = false; }
                    if (!state.customerCategoryId) { state.errors.customerCategoryId = 'Ügyfélkategória megadása kötelező.'; isValid = false; }

                    if (!isValid) return;

                    const userId = StorageManager.getUserId();

                    const payload = {
                        id: state.id,
                        name: state.name,
                        description: state.description,
                        contactPersonName: state.contactPersonName,
                        phoneNumber: state.phoneNumber,
                        faxNumber: state.faxNumber,
                        emailAddress: state.emailAddress,
                        emailAddressOrderConfirmation: state.emailAddressOrderConfirmation,
                        emailAddressInvoice: state.emailAddressInvoice,
                        emailAddressPurchaseOrder: state.emailAddressPurchaseOrder,
                        website: state.website,
                        taxNumber: state.taxNumber,
                        euTaxNumber: state.euTaxNumber,
                        bankAccountNumber: state.bankAccountNumber,
                        invoiceType: state.invoiceType,
                        paymentMethod: state.paymentMethod,
                        paymentDeadlineDays: state.paymentDeadlineDays,
                        paymentDeadline: state.paymentDeadlineDays,
                        currency: state.currency,
                        customerGroupId: state.customerGroupId,
                        customerCategoryId: state.customerCategoryId,
                        addresses: [{
                            street: state.street,
                            city: state.city,
                            zipCode: state.zipCode,
                            country: state.country,
                            type: 1
                        }],
                        createdById: userId,
                        updatedById: userId
                    };

                    const response = state.id === ''
                        ? await services.createMainData(payload)
                        : state.deleteMode
                            ? await services.deleteMainData(state.id, userId)
                            : await services.updateMainData(payload);

                    if (response.data.code === 200) {
                        await methods.populateMainData();
                        mainGrid.refresh();

                        if (!state.deleteMode && state.id === '') {
                            state.id = response?.data?.content?.data?.id ?? '';
                            state.number = response?.data?.content?.data?.number ?? '';
                            state.mainTitle = 'Ügyfél szerkesztése';
                        }

                        Swal.fire({
                            icon: 'success',
                            title: state.deleteMode ? 'Törölve' : 'Mentve',
                            text: 'Az ablak bezárul...',
                            timer: 2000,
                            showConfirmButton: false
                        });
                        setTimeout(() => {
                            mainModal.obj.hide();
                            resetFormState();
                        }, 2000);

                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: state.deleteMode ? 'Törlés sikertelen' : 'Mentés sikertelen',
                            text: response.data.message ?? 'Kérjük ellenőrizze az adatokat.',
                            confirmButtonText: 'Újra'
                        });
                    }

                } catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hiba történt',
                        text: error.response?.data?.message ?? 'Kérjük próbálja újra.',
                        confirmButtonText: 'OK'
                    });
                } finally {
                    state.isSubmitting = false;
                }
            }
        };

        const mainGrid = {
            obj: null,
            create: async (dataSource) => {
                mainGrid.obj = new ej.grids.Grid({
                    height: '400px',
                    dataSource: dataSource,
                    allowFiltering: true,
                    allowSorting: true,
                    allowSelection: true,
                    allowTextWrap: true,
                    allowResizing: true,
                    allowPaging: true,
                    allowExcelExport: true,
                    filterSettings: { type: 'CheckBox' },
                    sortSettings: { columns: [{ field: 'createdAtUtc', direction: 'Descending' }] },
                    pageSettings: { currentPage: 1, pageSize: 50, pageSizes: ['10', '20', '50', '100', 'All'] },
                    selectionSettings: { persistSelection: true, type: 'Single' },
                    showColumnMenu: true,
                    gridLines: 'Horizontal',
                    columns: [
                        { type: 'checkbox', width: 50 },
                        { field: 'id', isPrimaryKey: true, headerText: 'Id', visible: false },
                        { field: 'number', headerText: 'Kód', width: 120, minWidth: 100 },
                        { field: 'name', headerText: 'Név', width: 200, minWidth: 150 },
                        { field: 'customerGroupName', headerText: 'Csoport', width: 150 },
                        { field: 'customerCategoryName', headerText: 'Kategória', width: 150 },
                        { field: 'taxNumber', headerText: 'Adószám', width: 140 },
                        { field: 'contactPersonName', headerText: 'Kapcsolattartó', width: 160 },
                        { field: 'phoneNumber', headerText: 'Telefon', width: 140 },
                        { field: 'emailAddress', headerText: 'E-mail', width: 180 },
                        { field: 'city', headerText: 'Város', width: 130 },
                        { field: 'createdAtUtc', headerText: 'Létrehozva', width: 140, format: 'yyyy-MM-dd HH:mm' }
                    ],
                    toolbar: [
                        'ExcelExport', 'Search',
                        { type: 'Separator' },
                        { text: 'Hozzáadás', tooltipText: 'Hozzáadás', prefixIcon: 'e-add', id: 'AddCustom' },
                        { text: 'Szerkesztés', tooltipText: 'Szerkesztés', prefixIcon: 'e-edit', id: 'EditCustom' },
                        { text: 'Törlés', tooltipText: 'Törlés', prefixIcon: 'e-delete', id: 'DeleteCustom' },
                        { type: 'Separator' },
                        { text: 'Kapcsolattartó', tooltipText: 'Kapcsolattartó kezelése', id: 'ManageContactCustom' },
                    ],
                    dataBound: () => {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom', 'ManageContactCustom'], false);
                    },
                    rowSelected: () => {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom', 'ManageContactCustom'], true);
                    },
                    rowDeselected: () => {
                        if (mainGrid.obj.getSelectedRecords().length === 0)
                            mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom', 'ManageContactCustom'], false);
                    },
                    rowSelecting: () => {
                        if (mainGrid.obj.getSelectedRecords().length)
                            mainGrid.obj.clearSelection();
                    },
                    toolbarClick: async (args) => {
                        if (args.item.id === 'MainGrid_excelexport') {
                            mainGrid.obj.excelExport();
                        }

                        if (args.item.id === 'AddCustom') {
                            state.deleteMode = false;
                            state.mainTitle = 'Ügyfél hozzáadása';
                            resetFormState();
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom') {
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            if (!r) return;
                            state.deleteMode = false;
                            state.mainTitle = 'Ügyfél szerkesztése';
                            populateFormFromRecord(r);
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom') {
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            if (!r) return;
                            state.deleteMode = true;
                            state.mainTitle = 'Ügyfél törlése?';
                            state.id = r.id ?? '';
                            state.name = r.name ?? '';
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'ManageContactCustom') {
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            if (!r) return;
                            state.id = r.id ?? '';
                            state.manageContactTitle = 'Kapcsolattartó kezelése';
                            await methods.populateSecondaryData(state.id);
                            secondaryGrid.refresh();
                            manageContactModal.obj.show();
                        }
                    }
                });
                mainGrid.obj.appendTo(mainGridRef.value);
            },
            refresh: () => mainGrid.obj.setProperties({ dataSource: state.mainData })
        };

        const mainModal = {
            obj: null,
            create: () => {
                mainModal.obj = new bootstrap.Modal(mainModalRef.value, { backdrop: 'static', keyboard: false });
            }
        };

        const manageContactModal = {
            obj: null,
            create: () => {
                manageContactModal.obj = new bootstrap.Modal(manageContactModalRef.value, { backdrop: 'static', keyboard: false });
            }
        };

        const secondaryGrid = {
            obj: null,
            create: async (dataSource) => {
                secondaryGrid.obj = new ej.grids.Grid({
                    height: '300px',
                    dataSource: dataSource,
                    allowFiltering: true,
                    allowSorting: true,
                    allowSelection: true,
                    allowPaging: true,
                    editSettings: { allowAdding: true, allowEditing: true, allowDeleting: true, mode: 'Normal' },
                    pageSettings: { currentPage: 1, pageSize: 10 },
                    selectionSettings: { persistSelection: true, type: 'Single' },
                    toolbar: ['ExcelExport', 'Add', 'Edit', 'Delete', 'Update', 'Cancel', 'Search'],
                    columns: [
                        { field: 'id', isPrimaryKey: true, headerText: 'Id', visible: false },
                        { field: 'name', headerText: 'Név', width: 200, minWidth: 200, validationRules: { required: true } },
                        { field: 'jobTitle', headerText: 'Beosztás', width: 200, minWidth: 200, validationRules: { required: true } },
                        { field: 'phoneNumber', headerText: 'Telefon', width: 200, minWidth: 200, validationRules: { required: true } },
                        { field: 'emailAddress', headerText: 'E-mail', width: 200, minWidth: 200, validationRules: { required: true } },
                        { field: 'description', headerText: 'Megjegyzés', width: 400, minWidth: 400 },
                        { field: 'createdAtUtc', headerText: 'Létrehozva', width: 150, format: 'yyyy-MM-dd HH:mm' }
                    ],
                    dataBound: () => {
                        secondaryGrid.obj.toolbarModule.enableItems(['Edit', 'Delete'], false);
                    },
                    rowSelected: () => {
                        if (secondaryGrid.obj.getSelectedRecords().length == 1)
                            secondaryGrid.obj.toolbarModule.enableItems(['Edit', 'Delete'], true);
                    },
                    rowDeselected: () => {
                        if (secondaryGrid.obj.getSelectedRecords().length == 0)
                            secondaryGrid.obj.toolbarModule.enableItems(['Edit', 'Delete'], false);
                    },
                    rowSelecting: () => {
                        if (secondaryGrid.obj.getSelectedRecords().length)
                            secondaryGrid.obj.clearSelection();
                    },
                    actionComplete: async (args) => {
                        if (args.requestType === 'save' && args.action === 'add') {
                            await services.createSecondaryData(
                                args.data.name, args.data.jobTitle, args.data.phoneNumber,
                                args.data.emailAddress, args.data.description,
                                state.id, StorageManager.getUserId()
                            );
                            await methods.populateSecondaryData(state.id);
                            secondaryGrid.refresh();
                            Swal.fire({ icon: 'success', title: 'Mentve', timer: 2000, showConfirmButton: false });
                        }
                        if (args.requestType === 'save' && args.action === 'edit') {
                            await services.updateSecondaryData(
                                args.data.id, args.data.name, args.data.jobTitle, args.data.phoneNumber,
                                args.data.emailAddress, args.data.description,
                                state.id, StorageManager.getUserId()
                            );
                            await methods.populateSecondaryData(state.id);
                            secondaryGrid.refresh();
                            Swal.fire({ icon: 'success', title: 'Frissítve', timer: 2000, showConfirmButton: false });
                        }
                        if (args.requestType === 'delete') {
                            await services.deleteSecondaryData(args.data[0].id, StorageManager.getUserId());
                            await methods.populateSecondaryData(state.id);
                            secondaryGrid.refresh();
                            Swal.fire({ icon: 'success', title: 'Törölve', timer: 2000, showConfirmButton: false });
                        }
                    }
                });
                secondaryGrid.obj.appendTo(secondaryGridRef.value);
            },
            refresh: () => secondaryGrid.obj.setProperties({ dataSource: state.secondaryData })
        };

        Vue.onMounted(async () => {
            try {
                await SecurityManager.authorizePage(['Customers']);
                await SecurityManager.validateToken();

                await methods.populateCustomerGroupListLookupData();
                await methods.populateCustomerCategoryListLookupData();
                await methods.populateMainData();

                await mainGrid.create(state.mainData);
                await secondaryGrid.create(state.secondaryData);

                customerGroupDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.customerGroupListLookupData,
                    fields: { value: 'id', text: 'name' },
                    placeholder: 'Válasszon csoportot',
                    change: e => { state.customerGroupId = e.value; state.errors.customerGroupId = ''; }
                });
                customerGroupDropdown.appendTo(customerGroupIdRef.value);

                customerCategoryDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.customerCategoryListLookupData,
                    fields: { value: 'id', text: 'name' },
                    placeholder: 'Válasszon kategóriát',
                    change: e => { state.customerCategoryId = e.value; state.errors.customerCategoryId = ''; }
                });
                customerCategoryDropdown.appendTo(customerCategoryIdRef.value);

                invoiceTypeDropdown = new ej.dropdowns.DropDownList({
                    dataSource: invoiceTypeLookup,
                    fields: { value: 'value', text: 'text' },
                    placeholder: 'Válasszon',
                    change: e => state.invoiceType = e.value
                });
                invoiceTypeDropdown.appendTo(invoiceTypeRef.value);

                paymentMethodDropdown = new ej.dropdowns.DropDownList({
                    dataSource: paymentMethodLookup,
                    fields: { value: 'value', text: 'text' },
                    placeholder: 'Válasszon',
                    change: e => state.paymentMethod = e.value
                });
                paymentMethodDropdown.appendTo(paymentMethodRef.value);

                paymentDeadlineDaysDropdown = new ej.dropdowns.DropDownList({
                    dataSource: paymentDeadlineDaysLookup,
                    fields: { value: 'value', text: 'text' },
                    placeholder: 'Válasszon',
                    change: e => state.paymentDeadlineDays = e.value
                });
                paymentDeadlineDaysDropdown.appendTo(paymentDeadlineDaysRef.value);

                currencyDropdown = new ej.dropdowns.DropDownList({
                    dataSource: currencyLookup,
                    fields: { value: 'value', text: 'text' },
                    placeholder: 'Válasszon',
                    change: e => state.currency = e.value
                });
                currencyDropdown.appendTo(currencyRef.value);

                mainModal.create();
                manageContactModal.create();

            } catch (e) {
                console.error('page init error:', e);
            }
        });

        return {
            mainGridRef,
            mainModalRef,
            manageContactModalRef,
            secondaryGridRef,
            customerGroupIdRef,
            customerCategoryIdRef,
            invoiceTypeRef,
            paymentMethodRef,
            paymentDeadlineDaysRef,
            currencyRef,
            state,
            handler
        };
    }
};

Vue.createApp(App).mount('#app');
