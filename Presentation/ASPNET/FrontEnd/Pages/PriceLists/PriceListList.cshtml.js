const App = {
    setup() {
        const state = Vue.reactive({
            mainData: [],
            deleteMode: false,
            mainTitle: null,
            isSubmitting: false,

            id: '',
            productId: null,
            customerId: null,

            productListLookupData: [],
            customerListLookupData: [],

            errors: {
                productId: ''
            }
        });

        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);
        const productIdRef = Vue.ref(null);
        const customerIdRef = Vue.ref(null);

        let productDropdown = null;
        let customerDropdown = null;

        const resetFormState = () => {
            Object.assign(state, {
                id: '',
                productId: null,
                customerId: null,
                errors: { productId: '' }
            });
            if (productDropdown) productDropdown.value = null;
            if (customerDropdown) customerDropdown.value = null;
        };

        const populateFormFromRecord = (r) => {
            Object.assign(state, {
                id: r.id,
                productId: r.productId,
                customerId: r.customerId
            });
            if (productDropdown) productDropdown.value = state.productId;
            if (customerDropdown) customerDropdown.value = state.customerId;
        };

        const services = {
            getMainData: () => AxiosManager.get('/ProductCustomer/GetProductCustomerList', {}),
            getProducts: () => AxiosManager.get('/Product/GetProductList', {}),
            getCustomers: () => AxiosManager.get('/Customer/GetCustomerList', {}),
            createMainData: (payload) => AxiosManager.post('/ProductCustomer/CreateProductCustomer', payload),
            updateMainData: (payload) => AxiosManager.post('/ProductCustomer/UpdateProductCustomer', payload),
            deleteMainData: (id, deletedById) => AxiosManager.post('/ProductCustomer/DeleteProductCustomer', { id, deletedById })
        };

        const methods = {
            populateMainData: async () => {
                const response = await services.getMainData();
                state.mainData = response?.data?.content?.data ?? [];
            },
            populateLookups: async () => {
                const [products, customers] = await Promise.all([
                    services.getProducts(),
                    services.getCustomers()
                ]);
                state.productListLookupData = products?.data?.content?.data ?? [];
                state.customerListLookupData = customers?.data?.content?.data ?? [];
            }
        };

        const handler = {
            handleSubmit: async () => {
                try {
                    state.isSubmitting = true;
                    await new Promise(resolve => setTimeout(resolve, 200));

                    state.errors = { productId: '' };
                    let isValid = true;
                    if (!state.productId) { state.errors.productId = 'Kötelező mező.'; isValid = false; }
                    if (!isValid) return;

                    const userId = StorageManager.getUserId();
                    const payload = {
                        id: state.id || undefined,
                        productId: state.productId,
                        customerId: state.customerId,
                        createdById: userId,
                        updatedById: userId,
                        deletedById: userId
                    };

                    const response = state.id === ''
                        ? await services.createMainData(payload)
                        : state.deleteMode
                            ? await services.deleteMainData(state.id, userId)
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
            create: async (data) => {
                mainGrid.obj = new ej.grids.Grid({
                    height: '240px',
                    dataSource: data,
                    allowSorting: true,
                    allowFiltering: true,
                    allowSelection: true,
                    allowGrouping: true,
                    allowTextWrap: true,
                    allowResizing: true,
                    allowPaging: true,
                    allowExcelExport: true,
                    filterSettings: { type: 'CheckBox' },
                    sortSettings: { columns: [{ field: 'productName', direction: 'Ascending' }] },
                    pageSettings: { currentPage: 1, pageSize: 50, pageSizes: ['10', '20', '50', '100', '200', 'All'] },
                    selectionSettings: { persistSelection: true, type: 'Single' },
                    autoFit: true,
                    showColumnMenu: true,
                    gridLines: 'Horizontal',
                    columns: [
                        { type: 'checkbox', width: 60 },
                        { field: 'id', isPrimaryKey: true, headerText: 'Id', visible: false },
                        { field: 'productNumber', headerText: 'Cikkszám', width: 130, minWidth: 100 },
                        { field: 'productName', headerText: 'Termék', width: 200, minWidth: 150 },
                        { field: 'customerName', headerText: 'Vevő', width: 180, minWidth: 120 }
                    ],
                    toolbar: [
                        'ExcelExport', 'Search',
                        { type: 'Separator' },
                        { text: 'Hozzáadás', tooltipText: 'Hozzáadás', prefixIcon: 'e-add', id: 'AddCustom' },
                        { text: 'Szerkesztés', tooltipText: 'Szerkesztés', prefixIcon: 'e-edit', id: 'EditCustom' },
                        { text: 'Törlés', tooltipText: 'Törlés', prefixIcon: 'e-delete', id: 'DeleteCustom' }
                    ],
                    dataBound: () => {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom'], false);
                        mainGrid.obj.autoFitColumns(['productNumber', 'productName', 'customerName']);
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
                            state.mainTitle = 'Termék-vevő kapcsolat hozzáadása';
                            resetFormState();
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = false;
                            state.mainTitle = 'Termék-vevő kapcsolat módosítása';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = true;
                            state.mainTitle = 'Termék-vevő kapcsolat törlése?';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
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

        Vue.onMounted(async () => {
            try {
                await SecurityManager.authorizePage(['PriceLists']);
                await SecurityManager.validateToken();

                await methods.populateLookups();
                await methods.populateMainData();
                await mainGrid.create(state.mainData);
                mainModal.create();

                productDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.productListLookupData,
                    fields: { value: 'id', text: 'name' },
                    placeholder: 'Válassz terméket',
                    change: e => { state.productId = e.value; state.errors.productId = ''; }
                });
                productDropdown.appendTo(productIdRef.value);

                customerDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.customerListLookupData,
                    fields: { value: 'id', text: 'name' },
                    placeholder: 'Válassz vevőt',
                    change: e => { state.customerId = e.value; }
                });
                customerDropdown.appendTo(customerIdRef.value);

            } catch (e) {
                console.error('page init error:', e);
            }
        });

        return {
            mainGridRef, mainModalRef,
            productIdRef, customerIdRef,
            state, handler
        };
    }
};

Vue.createApp(App).mount('#app');
