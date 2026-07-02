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
            taxId: null,
            netPrice: 0,
            grossPrice: null,
            quantityDiscount: null,
            discountFrom: null,
            discountTo: null,

            productListLookupData: [],
            customerListLookupData: [],
            taxListLookupData: [],

            errors: {
                productId: '',
                netPrice: ''
            },

            importResult: {
                successCount: 0,
                errorCount: 0,
                overwriteCount: 0,
                errors: []
            }
        });

        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);
        const importResultModalRef = Vue.ref(null);
        const productIdRef = Vue.ref(null);
        const customerIdRef = Vue.ref(null);
        const taxIdRef = Vue.ref(null);

        let productDropdown = null;
        let customerDropdown = null;
        let taxDropdown = null;

        const resetFormState = () => {
            Object.assign(state, {
                id: '',
                productId: null,
                customerId: null,
                taxId: null,
                netPrice: 0,
                grossPrice: null,
                quantityDiscount: null,
                discountFrom: null,
                discountTo: null,
                errors: { productId: '', netPrice: '' }
            });
            if (productDropdown) productDropdown.value = null;
            if (customerDropdown) customerDropdown.value = null;
            if (taxDropdown) taxDropdown.value = null;
        };

        const populateFormFromRecord = (r) => {
            Object.assign(state, {
                id: r.id,
                productId: r.productId,
                customerId: r.customerId,
                taxId: r.taxId,
                netPrice: r.netPrice,
                grossPrice: r.grossPrice,
                quantityDiscount: r.quantityDiscount,
                discountFrom: r.discountFrom ? r.discountFrom.substring(0, 10) : null,
                discountTo: r.discountTo ? r.discountTo.substring(0, 10) : null
            });
            if (productDropdown) productDropdown.value = state.productId;
            if (customerDropdown) customerDropdown.value = state.customerId;
            if (taxDropdown) taxDropdown.value = state.taxId;
        };

        const services = {
            getMainData: () => AxiosManager.get('/PriceList/GetPriceList', {}),
            getProducts: () => AxiosManager.get('/Product/GetProductList', {}),
            getCustomers: () => AxiosManager.get('/Customer/GetCustomerList', {}),
            getTaxes: () => AxiosManager.get('/Tax/GetTaxList', {}),
            createMainData: (payload) => AxiosManager.post('/PriceList/CreatePriceList', payload),
            updateMainData: (payload) => AxiosManager.post('/PriceList/UpdatePriceList', payload),
            deleteMainData: (id, deletedById) => AxiosManager.post('/PriceList/DeletePriceList', { id, deletedById })
        };

        const methods = {
            populateMainData: async () => {
                const response = await services.getMainData();
                state.mainData = response?.data?.content?.data ?? [];
            },
            populateLookups: async () => {
                const [products, customers, taxes] = await Promise.all([
                    services.getProducts(),
                    services.getCustomers(),
                    services.getTaxes()
                ]);
                state.productListLookupData = products?.data?.content?.data ?? [];
                state.customerListLookupData = customers?.data?.content?.data ?? [];
                state.taxListLookupData = taxes?.data?.content?.data ?? [];
            }
        };

        const handler = {
            handleSubmit: async () => {
                try {
                    state.isSubmitting = true;
                    await new Promise(resolve => setTimeout(resolve, 200));

                    state.errors = { productId: '', netPrice: '' };
                    let isValid = true;
                    if (!state.productId) { state.errors.productId = 'Kötelező mező.'; isValid = false; }
                    if (state.netPrice === null || state.netPrice === '') { state.errors.netPrice = 'Kötelező mező.'; isValid = false; }
                    if (!isValid) return;

                    const userId = StorageManager.getUserId();
                    const payload = {
                        id: state.id || undefined,
                        productId: state.productId,
                        customerId: state.customerId,
                        taxId: state.taxId,
                        netPrice: parseFloat(state.netPrice) || 0,
                        grossPrice: state.grossPrice ? parseFloat(state.grossPrice) : null,
                        quantityDiscount: state.quantityDiscount ? parseFloat(state.quantityDiscount) : null,
                        discountFrom: state.discountFrom || null,
                        discountTo: state.discountTo || null,
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
                        { field: 'customerName', headerText: 'Vevő', width: 180, minWidth: 120 },
                        { field: 'taxName', headerText: 'ÁFA', width: 100, minWidth: 80 },
                        { field: 'netPrice', headerText: 'Nettó ár', width: 110, minWidth: 90, format: 'N2' },
                        { field: 'grossPrice', headerText: 'Bruttó ár', width: 110, minWidth: 90, format: 'N2' },
                        { field: 'quantityDiscount', headerText: 'Menny. kedv.', width: 130, minWidth: 100, format: 'N2' },
                        { field: 'discountFrom', headerText: 'Kedvezménytől', width: 150, minWidth: 120, type: 'date', format: 'yMd' },
                        { field: 'discountTo', headerText: 'Kedvezményig', width: 150, minWidth: 120, type: 'date', format: 'yMd' }
                    ],
                    toolbar: [
                        'ExcelExport', 'Search',
                        { type: 'Separator' },
                        { text: 'Hozzáadás', tooltipText: 'Hozzáadás', prefixIcon: 'e-add', id: 'AddCustom' },
                        { text: 'Szerkesztés', tooltipText: 'Szerkesztés', prefixIcon: 'e-edit', id: 'EditCustom' },
                        { text: 'Törlés', tooltipText: 'Törlés', prefixIcon: 'e-delete', id: 'DeleteCustom' },
                        { type: 'Separator' },
                        { text: 'Import', tooltipText: 'Excel import', prefixIcon: 'e-upload', id: 'ImportExcel' },
                        { text: 'Sablon', tooltipText: 'Sablon letöltése', prefixIcon: 'e-download', id: 'DownloadTemplate' }
                    ],
                    dataBound: () => {
                        mainGrid.obj.toolbarModule.enableItems(['EditCustom', 'DeleteCustom'], false);
                        mainGrid.obj.autoFitColumns(['productNumber', 'productName', 'customerName', 'taxName', 'netPrice', 'grossPrice']);
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
                            state.mainTitle = 'Árlista hozzáadása';
                            resetFormState();
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = false;
                            state.mainTitle = 'Árlista módosítása';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = true;
                            state.mainTitle = 'Árlista törlése?';
                            populateFormFromRecord(mainGrid.obj.getSelectedRecords()[0]);
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'ImportExcel') {
                            document.getElementById('excelImportInput').click();
                        }

                        if (args.item.id === 'DownloadTemplate') {
                            AxiosManager.getFile('/PriceList/GetPriceListImportTemplate', 'arlista-import-template.xlsx');
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

        const importResultModal = {
            obj: null,
            create: () => {
                importResultModal.obj = new bootstrap.Modal(importResultModalRef.value, { backdrop: 'static', keyboard: false });
            },
            show: () => importResultModal.obj.show()
        };

        Vue.onMounted(async () => {
            try {
                await SecurityManager.authorizePage(['PriceLists']);
                await SecurityManager.validateToken();

                await methods.populateLookups();
                await methods.populateMainData();
                await mainGrid.create(state.mainData);
                mainModal.create();
                importResultModal.create();

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

                taxDropdown = new ej.dropdowns.DropDownList({
                    dataSource: state.taxListLookupData,
                    fields: { value: 'id', text: 'name' },
                    placeholder: 'Válassz ÁFÁ-t',
                    change: e => { state.taxId = e.value; }
                });
                taxDropdown.appendTo(taxIdRef.value);

                document.getElementById('excelImportInput').addEventListener('change', async (e) => {
                    const file = e.target.files[0];
                    if (!file) return;
                    e.target.value = '';

                    const formData = new FormData();
                    formData.append('file', file);

                    try {
                        const result = await AxiosManager.postFile(
                            '/PriceList/ImportPriceListFromExcel',
                            formData,
                            `arlista-import-hibak-${new Date().toISOString().slice(0, 10)}.xlsx`
                        );
                        const content = result?.content ?? {};
                        state.importResult = {
                            successCount: content.successCount ?? 0,
                            errorCount: content.errorCount ?? 0,
                            overwriteCount: content.overwriteCount ?? 0,
                            errors: content.errors ?? []
                        };
                        importResultModal.show();
                        if ((content.successCount ?? 0) > 0 || (content.overwriteCount ?? 0) > 0) {
                            await methods.populateMainData();
                            mainGrid.refresh();
                        }
                    } catch (err) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Import hiba',
                            text: err?.response?.data?.message ?? err.message,
                            confirmButtonText: 'OK'
                        });
                    }
                });

            } catch (e) {
                console.error('page init error:', e);
            }
        });

        return {
            mainGridRef, mainModalRef, importResultModalRef,
            productIdRef, customerIdRef, taxIdRef,
            state, handler
        };
    }
};

Vue.createApp(App).mount('#app');
