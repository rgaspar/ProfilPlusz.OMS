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

        const services = {
            getMainData: () => AxiosManager.get('/Customer/GetCustomerList', {}),
            createMainData: payload => AxiosManager.post('/Customer/CreateCustomer', payload),
            updateMainData: payload => AxiosManager.post('/Customer/UpdateCustomer', payload),
            deleteMainData: (id, deletedById) => AxiosManager.post('/Customer/DeleteCustomer', { id, deletedById }),
            getCustomerGroupListLookupData: () => AxiosManager.get('/CustomerGroup/GetCustomerGroupList', {}),
            getCustomerCategoryListLookupData: () => AxiosManager.get('/CustomerCategory/GetCustomerCategoryList', {}),
            getSecondaryData: customerId =>
                AxiosManager.get(`/CustomerContact/GetCustomerContactByCustomerIdList?customerId=${customerId}`, {}),
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
        };

        let customerGroupDropdown = null;
        let customerCategoryDropdown = null;

        const handler = {
            handleSubmit: async function () {
                try {
                    state.isSubmitting = true;
                    await new Promise(resolve => setTimeout(resolve, 200));

                    state.errors = { name: '', emailAddress: '', customerGroupId: '', customerCategoryId: '' };
                    let isValid = true;

                    if (!state.name) { state.errors.name = 'Név megadása kötelező.'; isValid = false; }
                    if (!state.emailAddress) { state.errors.emailAddress = 'E-mail megadása kötelező.'; isValid = false; }
                    if (!state.customerGroupId) { state.errors.customerGroupId = 'Ügyfélcsoport megadása kötelező.'; isValid = false; }
                    if (!state.customerCategoryId) { state.errors.customerCategoryId = 'Ügyfélkategória megadása kötelező.'; isValid = false; }

                    if (!isValid) return;

                    const payload = {
                        id: state.id || undefined,
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
                        paymentDeadline: state.paymentDeadlineDays,
                        paymentDeadlineDays: state.paymentDeadlineDays,
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
                        createdById: StorageManager.getUserId(),
                        updatedById: StorageManager.getUserId(),
                        deletedById: StorageManager.getUserId(),
                    };

                    const response = state.id === ''
                        ? await services.createMainData(payload)
                        : state.deleteMode
                            ? await services.deleteMainData(state.id, StorageManager.getUserId())
                            : await services.updateMainData(payload);

                    if (response.data.code === 200) {
                        await methods.populateMainData();
                        mainGrid.refresh();
                        Swal.fire({
                            icon: 'success',
                            title: state.deleteMode ? 'Törölve' : 'Mentve',
                            timer: 2000,
                            showConfirmButton: false
                        });
                        setTimeout(() => { mainModal.obj.hide(); resetFormState(); }, 2000);
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: state.deleteMode ? 'Törlés sikertelen' : 'Mentés sikertelen',
                            text: response.data.message ?? 'Ellenőrizd az adatokat.',
                            confirmButtonText: 'Újra'
                        });
                    }
                } catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hiba történt',
                        text: error.response?.data?.message ?? 'Próbáld újra.',
                        confirmButtonText: 'OK'
                    });
                } finally {
                    state.isSubmitting = false;
                }
            },
        };

        const mainGrid = {
            obj: null,
            create: async (dataSource) => {
                mainGrid.obj = new ej.grids.Grid({
                    height: '240px',
                    dataSource: dataSource,
                    allowFiltering: true,
                    allowSorting: true,
                    allowSelection: true,
                    allowGrouping: true,
                    allowTextWrap: true,
                    allowResizing: true,
                    allowPaging: true,
                    allowExcelExport: true,
                    filterSettings: { type: 'CheckBox' },
                    sortSettings: { columns: [{ field: 'createdAtUtc', direction: 'Descending' }] },
                    pageSettings: { currentPage: 1, pageSize: 50, pageSizes: ['10', '20', '50', '100', '200', 'All'] },
                    selectionSettings: { persistSelection: true, type: 'Single' },
                    autoFit: true,
                    showColumnMenu: true,
                    gridLines: 'Horizontal',
                    columns: [
                        { type: 'checkbox', width: 60 },
                        { field: 'id', isPrimaryKey: true, headerText: 'Id', visible: false },
                        { field: 'number', headerText: 'Kód', width: 100, minWidth: 100 },
                        { field: 'name', headerText: 'Név', width: 200, minWidth: 150 },
                        { field: 'customerGroupName', headerText: 'Csoport', width: 150, minWidth: 100 },
                        { field: 'customerCategoryName', headerText: 'Kategória', width: 150, minWidth: 100 },
                        { field: 'taxNumber', headerText: 'Adószám', width: 130, minWidth: 100 },
                        { field: 'contactPersonName', headerText: 'Kapcsolattartó', width: 150, minWidth: 100 },
                        { field: 'phoneNumber', headerText: 'Telefon', width: 130, minWidth: 100 },
                        { field: 'emailAddress', headerText: 'E-mail', width: 200, minWidth: 150 },
                        { field: 'city', headerText: 'Város', width: 120, minWidth: 100 },
                        { field: 'createdAtUtc', headerText: 'Létrehozva', width: 150, format: 'yyyy-MM-dd HH:mm' }
                    ],
                    toolbar: [
                        'ExcelExport', 'Search',
                        { type: 'Separator' },
                        { text: 'Hozzáadás', tooltipText: 'Hozzáadás', prefixIcon: 'e-add', id: 'AddCustom' },
                        { text: 'Szerkesztés', tooltipText: 'Szerkesztés', prefixIcon: 'e-edit', id: 'EditCustom' },
                        { text: 'Törlés', tooltipText: 'Törlés', prefixIcon: 'e-delete', id: 'DeleteCustom' },
                        { type: 'Separator' },
                    ],
                    dataBound: function () {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom'], false);
                        mainGrid.obj.autoFitColumns(['number', 'name', 'customerGroupName', 'customerCategoryName', 'emailAddress', 'createdAtUtc']);
                    },
                    rowSelected: () => {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom'], mainGrid.obj.getSelectedRecords().length === 1);
                    },
                    rowDeselected: () => {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom'], mainGrid.obj.getSelectedRecords().length === 1);
                    },
                    rowSelecting: () => {
                        if (mainGrid.obj.getSelectedRecords().length) mainGrid.obj.clearSelection();
                    },
                    toolbarClick: async (args) => {
                        if (args.item.id === 'MainGrid_excelexport') mainGrid.obj.excelExport();

                        if (args.item.id === 'AddCustom') {
                            state.deleteMode = false;
                            state.mainTitle = 'Vevő hozzáadása';
                            resetFormState();
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = false;
                            state.mainTitle = 'Vevő módosítása';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = true;
                            state.mainTitle = 'Vevő törlése?';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
                        }
                    }
                });
                mainGrid.obj.appendTo(mainGridRef.value);
            },
            refresh: () => {
                mainGrid.obj.setProperties({ dataSource: state.mainData });
            }
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
                    height: '240px',
                    dataSource: dataSource,
                    allowFiltering: true,
                    allowSorting: true,
                    allowPaging: true,
                    allowResizing: true,
                    filterSettings: { type: 'CheckBox' },
                    pageSettings: { currentPage: 1, pageSize: 20 },
                    gridLines: 'Horizontal',
                    columns: [
                        { field: 'id', isPrimaryKey: true, headerText: 'Id', visible: false },
                        { field: 'name', headerText: 'Név', width: 180 },
                        { field: 'jobTitle', headerText: 'Beosztás', width: 150 },
                        { field: 'phoneNumber', headerText: 'Telefon', width: 130 },
                        { field: 'emailAddress', headerText: 'E-mail', width: 200 },
                    ],
                });
                secondaryGrid.obj.appendTo(secondaryGridRef.value);
            }
        };

        Vue.onMounted(async () => {
            try {
                await SecurityManager.authorizePage(['Customers']);
                await SecurityManager.validateToken();

                await Promise.all([
                    methods.populateMainData(),
                    methods.populateCustomerGroupListLookupData(),
                    methods.populateCustomerCategoryListLookupData(),
                ]);

                await mainGrid.create(state.mainData);
                mainModal.create();
                manageContactModal.create();
                await secondaryGrid.create([]);

                customerGroupDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.customerGroupListLookupData,
                    fields: { text: 'name', value: 'id' },
                    placeholder: 'Válassz csoportot',
                    change: (e) => { state.customerGroupId = e.value; state.errors.customerGroupId = ''; }
                });
                customerGroupDropdown.appendTo(customerGroupIdRef.value);

                customerCategoryDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.customerCategoryListLookupData,
                    fields: { text: 'name', value: 'id' },
                    placeholder: 'Válassz kategóriát',
                    change: (e) => { state.customerCategoryId = e.value; state.errors.customerCategoryId = ''; }
                });
                customerCategoryDropdown.appendTo(customerCategoryIdRef.value);

            } catch (e) {
                console.error('page init error:', e);
            }
        });

        return {
            mainGridRef, mainModalRef, manageContactModalRef,
            secondaryGridRef, customerGroupIdRef, customerCategoryIdRef,
            state, handler,
        };
    }
};

Vue.createApp(App).mount('#app');
