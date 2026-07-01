const App = {
    setup() {
        const state = Vue.reactive({
            mainData: [],
            deleteMode: false,
            customerListLookupData: [],
            mainTitle: null,
            id: '',
            number: '',
            name: '',
            jobTitle: '',
            phoneNumber: '',
            emailAddress: '',
            description: '',
            customerId: null,
            errors: {
                name: '',
                jobTitle: '',
                phoneNumber: '',
                emailAddress: '',
                customerId: ''
            },
            isSubmitting: false
        });

        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);
        const customerIdRef = Vue.ref(null);

        let customerDropdown = null;

        const resetFormState = () => {
            Object.assign(state, {
                id: '',
                number: '',
                name: '',
                jobTitle: '',
                phoneNumber: '',
                emailAddress: '',
                description: '',
                customerId: null,
                errors: { name: '', jobTitle: '', phoneNumber: '', emailAddress: '', customerId: '' }
            });
            if (customerDropdown) customerDropdown.value = null;
        };

        const populateFormFromRecord = (r) => {
            Object.assign(state, {
                id: r.id ?? '',
                number: r.number ?? '',
                name: r.name ?? '',
                jobTitle: r.jobTitle ?? '',
                phoneNumber: r.phoneNumber ?? '',
                emailAddress: r.emailAddress ?? '',
                description: r.description ?? '',
                customerId: r.customerId ?? null,
            });
            if (customerDropdown) customerDropdown.value = state.customerId;
        };

        const services = {
            getMainData: () => AxiosManager.get('/CustomerContact/GetCustomerContactList', {}),
            createMainData: (payload) => AxiosManager.post('/CustomerContact/CreateCustomerContact', payload),
            updateMainData: (payload) => AxiosManager.post('/CustomerContact/UpdateCustomerContact', payload),
            deleteMainData: (id, deletedById) => AxiosManager.post('/CustomerContact/DeleteCustomerContact', { id, deletedById }),
            getCustomerList: () => AxiosManager.get('/Customer/GetCustomerList', {}),
        };

        const methods = {
            populateMainData: async () => {
                const response = await services.getMainData();
                state.mainData = (response?.data?.content?.data ?? []).map(item => ({
                    ...item,
                    createdAtUtc: new Date(item.createdAtUtc)
                }));
            },
            populateCustomerList: async () => {
                const response = await services.getCustomerList();
                state.customerListLookupData = response?.data?.content?.data ?? [];
            },
        };

        const handler = {
            handleSubmit: async function () {
                try {
                    state.isSubmitting = true;
                    await new Promise(resolve => setTimeout(resolve, 200));

                    state.errors = { name: '', jobTitle: '', phoneNumber: '', emailAddress: '', customerId: '' };
                    let isValid = true;

                    if (!state.name) { state.errors.name = 'Kötelező mező.'; isValid = false; }
                    if (!state.jobTitle) { state.errors.jobTitle = 'Kötelező mező.'; isValid = false; }
                    if (!state.phoneNumber) { state.errors.phoneNumber = 'Kötelező mező.'; isValid = false; }
                    if (!state.emailAddress) { state.errors.emailAddress = 'Kötelező mező.'; isValid = false; }
                    if (!state.customerId) { state.errors.customerId = 'Kötelező mező.'; isValid = false; }

                    if (!isValid) return;

                    const payload = {
                        id: state.id || undefined,
                        name: state.name,
                        jobTitle: state.jobTitle,
                        phoneNumber: state.phoneNumber,
                        emailAddress: state.emailAddress,
                        description: state.description,
                        customerId: state.customerId,
                        createdById: StorageManager.getUserId(),
                        updatedById: StorageManager.getUserId(),
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
            }
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
                        { field: 'name', headerText: 'Név', width: 180, minWidth: 150 },
                        { field: 'customerName', headerText: 'Ügyfél', width: 150, minWidth: 120 },
                        { field: 'jobTitle', headerText: 'Munkakör', width: 150, minWidth: 120 },
                        { field: 'phoneNumber', headerText: 'Telefon', width: 130, minWidth: 100 },
                        { field: 'emailAddress', headerText: 'E-mail', width: 200, minWidth: 150 },
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
                        mainGrid.obj.autoFitColumns(['number', 'name', 'customerName', 'jobTitle', 'emailAddress', 'createdAtUtc']);
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
                            state.mainTitle = 'Kapcsolattartó hozzáadása';
                            resetFormState();
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = false;
                            state.mainTitle = 'Kapcsolattartó módosítása';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = true;
                            state.mainTitle = 'Kapcsolattartó törlése?';
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

        Vue.onMounted(async () => {
            try {
                await SecurityManager.authorizePage(['CustomerContacts']);
                await SecurityManager.validateToken();

                await Promise.all([
                    methods.populateMainData(),
                    methods.populateCustomerList(),
                ]);

                await mainGrid.create(state.mainData);
                mainModal.create();

                customerDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.customerListLookupData,
                    fields: { text: 'name', value: 'id' },
                    placeholder: 'Válassz ügyfelet',
                    change: (e) => { state.customerId = e.value; state.errors.customerId = ''; }
                });
                customerDropdown.appendTo(customerIdRef.value);

            } catch (e) {
                console.error('page init error:', e);
            }
        });

        return {
            mainGridRef, mainModalRef, customerIdRef,
            state, handler,
        };
    }
};

Vue.createApp(App).mount('#app');
