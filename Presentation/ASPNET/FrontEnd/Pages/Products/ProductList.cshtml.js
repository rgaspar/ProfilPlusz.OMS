const App = {

    setup() {

        const state = Vue.reactive({

            mainData: [],

            deleteMode: false,

            mainTitle: null,

            id: '',

            name: '',
            number: '',
            description: '',

            unitPrice: 0,
            physical: true,

            productGroupId: null,
            unitMeasureId: null,

            factoryName: '',
            manufacturer: '',
            manufacturerNumber: '',
            ean: '',

            brandId: null,
            colorId: null,

            salesUnitQuantity: null,
            minimumSalesQuantity: null,
            orderQuantityStep: null,
            packageQuantity: null,
            weight: null,

            isStockProduct: true,
            warningStock: null,
            minimumStock: null,
            hasSerialNumber: false,

            image1Url: '',
            image2Url: '',
            image3Url: '',

            videoUrl: '',
            pdfUrl: '',

            purchaseCurrency: 'EUR',
            salesCurrency: 'HUF',

            status: 'Active',

            productGroupListLookupData: [],
            unitMeasureListLookupData: [],
            brandListLookupData: [],
            colorListLookupData: [],

            currencyListLookupData: [
                { value: 'HUF', text: 'HUF' },
                { value: 'EUR', text: 'EUR' },
                { value: 'USD', text: 'USD' }
            ],

            statusListLookupData: [
                { value: 'Active', text: 'Active' },
                { value: 'Inactive', text: 'Inactive' },
                { value: 'Blocked', text: 'Blocked' }
            ],

            errors: {

                name: '',
                unitPrice: '',
                productGroupId: '',
                unitMeasureId: ''

            },

            isSubmitting: false,

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

        const productGroupIdRef = Vue.ref(null);
        const unitMeasureIdRef = Vue.ref(null);
        const brandIdRef = Vue.ref(null);
        const colorIdRef = Vue.ref(null);

        const purchaseCurrencyRef = Vue.ref(null);
        const salesCurrencyRef = Vue.ref(null);
        const statusRef = Vue.ref(null);

        const nameRef = Vue.ref(null);
        const numberRef = Vue.ref(null);
        const unitPriceRef = Vue.ref(null);



        const validateForm = () => {

            state.errors.name = '';
            state.errors.unitPrice = '';
            state.errors.productGroupId = '';
            state.errors.unitMeasureId = '';

            let valid = true;

            if (!state.name) {

                state.errors.name = 'Required';
                valid = false;

            }

            if (!state.unitPrice) {

                state.errors.unitPrice = 'Required';
                valid = false;

            }

            if (!state.productGroupId) {

                state.errors.productGroupId = 'Required';
                valid = false;

            }

            if (!state.unitMeasureId) {

                state.errors.unitMeasureId = 'Required';
                valid = false;

            }

            return valid;

        };



        const resetFormState = () => {

            Object.assign(state, {

                id: '',

                name: '',
                number: '',
                description: '',

                unitPrice: 0,
                physical: true,

                productGroupId: null,
                unitMeasureId: null,

                factoryName: '',
                manufacturer: '',
                manufacturerNumber: '',
                ean: '',

                brandId: null,
                colorId: null,

                salesUnitQuantity: null,
                minimumSalesQuantity: null,
                orderQuantityStep: null,
                packageQuantity: null,
                weight: null,

                isStockProduct: true,
                warningStock: null,
                minimumStock: null,
                hasSerialNumber: false,

                image1Url: '',
                image2Url: '',
                image3Url: '',

                videoUrl: '',
                pdfUrl: '',

                purchaseCurrency: 'EUR',
                salesCurrency: 'HUF',

                status: 'Active'
            });

        };



        const services = {

            getMainData: () =>
                AxiosManager.get('/Product/GetProductList'),

            createMainData: data =>
                AxiosManager.post('/Product/CreateProduct', data),

            updateMainData: data =>
                AxiosManager.post('/Product/UpdateProduct', data),

            deleteMainData: (id, userId) =>
                AxiosManager.post('/Product/DeleteProduct', {
                    id,
                    deletedById: userId
                }),

            getProductGroupListLookupData: () =>
                AxiosManager.get('/ProductGroup/GetProductGroupList'),

            getUnitMeasureListLookupData: () =>
                AxiosManager.get('/UnitMeasure/GetUnitMeasureList'),

            getBrandList: () =>
                AxiosManager.get('/Brand/GetBrandList'),

            getColorList: () =>
                AxiosManager.get('/Color/GetColorList')

        };



        const methods = {

            populateMainData: async () => {

                const response = await services.getMainData();

                state.mainData = response.data.content.data.map(x => ({

                    ...x,

                    createdAtUtc: new Date(x.createdAtUtc)

                }));

            },

            populateLookups: async () => {

                state.productGroupListLookupData =
                    (await services.getProductGroupListLookupData())
                        .data.content.data;

                state.unitMeasureListLookupData =
                    (await services.getUnitMeasureListLookupData())
                        .data.content.data;

                state.brandListLookupData =
                    (await services.getBrandList())
                        .data.content.data;

                state.colorListLookupData =
                    (await services.getColorList())
                        .data.content.data;

            }

        };



        const createDropdown = (ref, data, value, text, changeFn) => {

            const obj = new ej.dropdowns.DropDownList({

                dataSource: data,

                fields: {

                    value: value,
                    text: text

                },

                change: e => changeFn(e.value)

            });

            obj.appendTo(ref.value);

            return obj;

        };



        const handler = {

            handleSubmit: async () => {

                try {

                    state.isSubmitting = true;

                    if (!validateForm())
                        return;


                    const payload = {

                        ...state,

                        createdById:
                            StorageManager.getUserId(),

                        updatedById:
                            StorageManager.getUserId()

                    };


                    const response =
                        state.id === ''

                            ? await services.createMainData(payload)

                            : state.deleteMode

                                ? await services.deleteMainData(

                                    state.id,
                                    StorageManager.getUserId()

                                )

                                : await services.updateMainData(payload);


                    if (response.data.code === 200) {

                        await methods.populateMainData();

                        mainGrid.refresh();


                        Swal.fire({

                            icon: 'success',
                            title: state.deleteMode
                                ? 'Deleted'
                                : 'Saved',

                            timer: 1500,
                            showConfirmButton: false

                        });


                        mainModal.obj.hide();

                    }

                }
                catch (e) {

                    Swal.fire({

                        icon: 'error',
                        title: 'Error',
                        text: e.message

                    });

                }
                finally {

                    state.isSubmitting = false;

                }

            }

        };



        const mainGrid = {

            obj: null,

            create: async dataSource => {

                mainGrid.obj =
                    new ej.grids.Grid({

                        height: '400px',

                        dataSource: dataSource,

                        allowFiltering: true,
                        allowSorting: true,
                        allowGrouping: true,
                        allowPaging: true,
                        allowExcelExport: true,

                        allowSelection: true,

                        filterSettings: {
                            type: 'CheckBox'
                        },

                        selectionSettings: {

                            persistSelection: true,
                            type: 'Single'

                        },

                        pageSettings: {

                            pageSize: 50

                        },

                        columns: [

                            {
                                type: 'checkbox',
                                width: 50
                            },

                            {
                                field: 'id',
                                isPrimaryKey: true,
                                visible: false
                            },

                            {
                                field: 'number',
                                headerText: 'Number',
                                width: 150
                            },

                            {
                                field: 'name',
                                headerText: 'Name',
                                width: 200
                            },

                            {
                                field: 'productGroupName',
                                headerText: 'Group',
                                width: 150
                            },

                            {
                                field: 'brandName',
                                headerText: 'Brand',
                                width: 150
                            },

                            {
                                field: 'colorName',
                                headerText: 'Color',
                                width: 150
                            },

                            {
                                field: 'unitPrice',
                                headerText: 'Price',
                                format: 'N2',
                                width: 120
                            },

                            {
                                field: 'status',
                                headerText: 'Status',
                                width: 120
                            },

                            {
                                field: 'createdAtUtc',
                                headerText: 'Created',
                                format: 'yyyy-MM-dd HH:mm',
                                width: 150
                            }

                        ],



                        toolbar: [

                            'ExcelExport',

                            'Search',

                            { type: 'Separator' },

                            {

                                text: 'Add',
                                id: 'AddCustom',
                                prefixIcon: 'e-add'

                            },

                            {

                                text: 'Edit',
                                id: 'EditCustom',
                                prefixIcon: 'e-edit'

                            },

                            {

                                text: 'Delete',
                                id: 'DeleteCustom',
                                prefixIcon: 'e-delete'

                            },

                            { type: 'Separator' },

                            {

                                text: 'Import Excel',
                                id: 'ImportExcel',
                                prefixIcon: 'e-upload'

                            },

                            {

                                text: 'Template',
                                id: 'DownloadTemplate',
                                prefixIcon: 'e-download'

                            }

                        ],



                        dataBound: () => {

                            mainGrid.obj
                                .toolbarModule
                                .enableItems(

                                    ['EditCustom', 'DeleteCustom'],
                                    false

                                );

                        },



                        rowSelected: () => {

                            mainGrid.obj
                                .toolbarModule
                                .enableItems(

                                    ['EditCustom', 'DeleteCustom'],
                                    true

                                );

                        },



                        rowDeselected: () => {

                            mainGrid.obj
                                .toolbarModule
                                .enableItems(

                                    ['EditCustom', 'DeleteCustom'],
                                    false

                                );

                        },



                        toolbarClick: args => {

                            if (args.item.id === 'MainGrid_excelexport') {

                                mainGrid.obj.excelExport();

                            }



                            if (args.item.id === 'AddCustom') {

                                resetFormState();

                                state.deleteMode = false;

                                state.mainTitle = 'Add Product';

                                mainModal.obj.show();

                            }



                            if (args.item.id === 'EditCustom') {

                                const r =
                                    mainGrid.obj.getSelectedRecords()[0];

                                if (!r)
                                    return;

                                state.deleteMode = false;

                                state.mainTitle = 'Edit Product';

                                Object.assign(state, r);

                                mainModal.obj.show();

                            }



                            if (args.item.id === 'DeleteCustom') {

                                const r =
                                    mainGrid.obj.getSelectedRecords()[0];

                                if (!r)
                                    return;

                                state.deleteMode = true;

                                state.id = r.id;

                                state.name = r.name;

                                state.mainTitle = 'Delete Product?';

                                mainModal.obj.show();

                            }

                            if (args.item.id === 'ImportExcel') {

                                document.getElementById('excelImportInput').click();

                            }

                            if (args.item.id === 'DownloadTemplate') {

                                AxiosManager.getFile('/Product/GetProductImportTemplate', 'termek-import-template.xlsx');

                            }

                        }

                    });


                mainGrid.obj.appendTo(mainGridRef.value);

            },


            refresh: () =>

                mainGrid.obj.setProperties({

                    dataSource: state.mainData

                })

        };



        const mainModal = {

            obj: null,

            create: () => {

                mainModal.obj =
                    new bootstrap.Modal(

                        mainModalRef.value,

                        {

                            backdrop: 'static',
                            keyboard: false

                        });

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

            await SecurityManager.authorizePage(['Products']);

            await SecurityManager.validateToken();


            await methods.populateLookups();

            await methods.populateMainData();

            await mainGrid.create(state.mainData);



            createDropdown(
                productGroupIdRef,
                state.productGroupListLookupData,
                'id',
                'name',
                v => state.productGroupId = v
            );


            createDropdown(
                unitMeasureIdRef,
                state.unitMeasureListLookupData,
                'id',
                'name',
                v => state.unitMeasureId = v
            );


            createDropdown(
                brandIdRef,
                state.brandListLookupData,
                'id',
                'name',
                v => state.brandId = v
            );


            createDropdown(
                colorIdRef,
                state.colorListLookupData,
                'id',
                'name',
                v => state.colorId = v
            );


            createDropdown(
                purchaseCurrencyRef,
                state.currencyListLookupData,
                'value',
                'text',
                v => state.purchaseCurrency = v
            );


            createDropdown(
                salesCurrencyRef,
                state.currencyListLookupData,
                'value',
                'text',
                v => state.salesCurrency = v
            );


            createDropdown(
                statusRef,
                state.statusListLookupData,
                'value',
                'text',
                v => state.status = v
            );


            mainModal.create();
            importResultModal.create();

            document.getElementById('excelImportInput').addEventListener('change', async (e) => {
                const file = e.target.files[0];
                if (!file) return;
                e.target.value = '';

                const formData = new FormData();
                formData.append('file', file);

                try {
                    const result = await AxiosManager.postFile(
                        '/Product/ImportProductsFromExcel',
                        formData,
                        `import-hibak-${new Date().toISOString().slice(0, 10)}.xlsx`
                    );

                    const content = result?.content ?? {};
                    state.importResult = {
                        successCount: content.successCount ?? 0,
                        errorCount: content.errorCount ?? 0,
                        overwriteCount: content.overwriteCount ?? 0,
                        errors: content.errors ?? []
                    };
                    importResultModal.show();

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
            importResultModalRef,

            productGroupIdRef,
            unitMeasureIdRef,
            brandIdRef,
            colorIdRef,

            purchaseCurrencyRef,
            salesCurrencyRef,
            statusRef,

            nameRef,
            numberRef,
            unitPriceRef,

            state,
            handler

        };

    }

};

Vue.createApp(App).mount('#app');