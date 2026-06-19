const App = {

    setup() {

        const state = Vue.reactive({

            mainData: [],

            deleteMode: false,

            mainTitle: null,

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

            isSubmitting: false

        });


        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);

        const productIdRef = Vue.ref(null);
        const customerIdRef = Vue.ref(null);
        const taxIdRef = Vue.ref(null);


        const validateForm = () => {

            state.errors.productId = '';
            state.errors.netPrice = '';

            let valid = true;

            if (!state.productId) {
                state.errors.productId = 'Required';
                valid = false;
            }

            if (state.netPrice === null || state.netPrice === '') {
                state.errors.netPrice = 'Required';
                valid = false;
            }

            return valid;

        };


        const resetFormState = () => {

            state.id = '';
            state.productId = null;
            state.customerId = null;
            state.taxId = null;
            state.netPrice = 0;
            state.grossPrice = null;
            state.quantityDiscount = null;
            state.discountFrom = null;
            state.discountTo = null;

            state.errors = { productId: '', netPrice: '' };

        };


        const services = {

            getMainData: async () => {
                return await AxiosManager.get('/PriceList/GetPriceList', {});
            },

            getProducts: async () => {
                return await AxiosManager.get('/Product/GetProductList', {});
            },

            getCustomers: async () => {
                return await AxiosManager.get('/Customer/GetCustomerList', {});
            },

            getTaxes: async () => {
                return await AxiosManager.get('/Tax/GetTaxList', {});
            },

            createMainData: async (payload) => {
                return await AxiosManager.post('/PriceList/CreatePriceList', payload);
            },

            updateMainData: async (payload) => {
                return await AxiosManager.post('/PriceList/UpdatePriceList', payload);
            },

            deleteMainData: async (id, deletedById) => {
                return await AxiosManager.post('/PriceList/DeletePriceList', { id, deletedById });
            }

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

                    if (!validateForm())
                        return;

                    const userId = StorageManager.getUserId();

                    const payload = {
                        id: state.id,
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

                    let response;

                    if (state.id === '') {
                        response = await services.createMainData(payload);
                    } else if (state.deleteMode) {
                        response = await services.deleteMainData(state.id, userId);
                    } else {
                        response = await services.updateMainData(payload);
                    }

                    await methods.populateMainData();
                    mainGrid.refresh();

                    mainModal.obj.hide();

                } catch (error) {

                    alert('Hiba: ' + (error?.response?.data?.message ?? error.message));

                } finally {

                    state.isSubmitting = false;

                }

            }

        };


        const createDropdown = (ref, data, value, text, changeFn) => {

            const obj = new ej.dropdowns.DropDownList({
                dataSource: data,
                fields: { value: value, text: text },
                change: e => changeFn(e.value)
            });

            obj.appendTo(ref.value);

            return obj;

        };


        const mainGrid = {

            obj: null,

            create: async (data) => {

                mainGrid.obj = new ej.grids.Grid({

                    dataSource: data,

                    allowSorting: true,
                    allowFiltering: true,
                    allowPaging: true,
                    allowExcelExport: true,

                    filterSettings: { type: 'CheckBox' },

                    pageSettings: { pageSize: 20 },

                    columns: [
                        { field: 'productNumber', headerText: 'Cikkszám', width: 130 },
                        { field: 'productName', headerText: 'Termék', width: 200 },
                        { field: 'customerName', headerText: 'Vevő', width: 180 },
                        { field: 'taxName', headerText: 'ÁFA', width: 100 },
                        { field: 'netPrice', headerText: 'Nettó ár', width: 110, format: 'N2' },
                        { field: 'grossPrice', headerText: 'Bruttó ár', width: 110, format: 'N2' },
                        { field: 'quantityDiscount', headerText: 'Menny. kedv.', width: 130, format: 'N2' },
                        { field: 'discountFrom', headerText: 'Kedvezménytől', width: 150, type: 'date', format: 'yMd' },
                        { field: 'discountTo', headerText: 'Kedvezményig', width: 150, type: 'date', format: 'yMd' }
                    ],

                    toolbar: [

                        'ExcelExport',
                        'Search',

                        { type: 'Separator' },

                        { text: 'Add', id: 'AddCustom', prefixIcon: 'e-add' },
                        { text: 'Edit', id: 'EditCustom', prefixIcon: 'e-edit' },
                        { text: 'Delete', id: 'DeleteCustom', prefixIcon: 'e-delete' },

                        { type: 'Separator' },

                        { text: 'Import Excel', id: 'ImportExcel', prefixIcon: 'e-upload' },
                        { text: 'Template', id: 'DownloadTemplate', prefixIcon: 'e-download' }

                    ],

                    dataBound: () => {

                        mainGrid.obj
                            .toolbarModule
                            .enableItems(['EditCustom', 'DeleteCustom'], false);

                    },

                    rowSelected: () => {

                        mainGrid.obj
                            .toolbarModule
                            .enableItems(['EditCustom', 'DeleteCustom'], true);

                    },

                    rowDeselected: () => {

                        mainGrid.obj
                            .toolbarModule
                            .enableItems(['EditCustom', 'DeleteCustom'], false);

                    },

                    toolbarClick: args => {

                        if (args.item.id === 'MainGrid_excelexport') {
                            mainGrid.obj.excelExport();
                        }

                        if (args.item.id === 'AddCustom') {
                            resetFormState();
                            state.deleteMode = false;
                            state.mainTitle = 'Add Price List';
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom') {
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            if (!r) return;
                            state.deleteMode = false;
                            state.mainTitle = 'Edit Price List';
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
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom') {
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            if (!r) return;
                            state.deleteMode = true;
                            state.id = r.id;
                            state.mainTitle = 'Delete Price List?';
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

                mainModal.obj = new bootstrap.Modal(
                    mainModalRef.value,
                    { backdrop: 'static', keyboard: false }
                );

            }

        };


        Vue.onMounted(async () => {

            await SecurityManager.authorizePage(['PriceLists']);

            await SecurityManager.validateToken();

            await methods.populateLookups();
            await methods.populateMainData();
            await mainGrid.create(state.mainData);

            createDropdown(
                productIdRef,
                state.productListLookupData,
                'id',
                'name',
                v => state.productId = v
            );

            createDropdown(
                customerIdRef,
                state.customerListLookupData,
                'id',
                'name',
                v => state.customerId = v
            );

            createDropdown(
                taxIdRef,
                state.taxListLookupData,
                'id',
                'name',
                v => state.taxId = v
            );

            mainModal.create();

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

                    if (result) {
                        alert(`Import kész: ${result.content?.successCount ?? 0} sor sikeresen betöltve.`);
                    } else {
                        alert('Import kész. Hibák a letöltött fájlban.');
                    }

                    await methods.populateMainData();
                    mainGrid.refresh();

                } catch (err) {
                    alert('Import hiba: ' + (err?.response?.data?.message ?? err.message));
                }

            });

        });


        return {

            mainGridRef,
            mainModalRef,

            productIdRef,
            customerIdRef,
            taxIdRef,

            state,
            handler

        };

    }

};

Vue.createApp(App).mount('#app');
