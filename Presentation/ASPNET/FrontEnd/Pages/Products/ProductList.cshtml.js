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
            length: null,

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
            },

            selectedProductId: null,
            selectedProductName: null,

            priceListData: [],
            customerListLookupData: [],
            taxListLookupData: [],

            plId: '',
            plProductId: null,
            plCustomerId: null,
            plTaxId: null,
            plNetPrice: 0,
            plGrossPrice: null,
            plQuantityDiscount: null,
            plDiscountFrom: null,
            plDiscountTo: null,
            plDeleteMode: false,
            plEditTitle: '',
            plIsSubmitting: false

        });



        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);
        const importResultModalRef = Vue.ref(null);
        const priceListModalRef = Vue.ref(null);
        const priceListEditModalRef = Vue.ref(null);
        const priceListGridRef = Vue.ref(null);
        const plCustomerIdRef = Vue.ref(null);
        const plTaxIdRef = Vue.ref(null);

        const productGroupIdRef = Vue.ref(null);
        const unitMeasureIdRef = Vue.ref(null);

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

                state.errors.name = 'Kötelező mező.';
                valid = false;

            }

            if (!state.unitPrice) {

                state.errors.unitPrice = 'Kötelező mező.';
                valid = false;

            }

            if (!state.productGroupId) {

                state.errors.productGroupId = 'Kötelező mező.';
                valid = false;

            }

            if (!state.unitMeasureId) {

                state.errors.unitMeasureId = 'Kötelező mező.';
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
                length: null,

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
                AxiosManager.get('/Color/GetColorList'),

            getPriceListByProduct: productId =>
                AxiosManager.get(`/PriceList/GetPriceList?productId=${encodeURIComponent(productId)}`),

            updatePriceList: data =>
                AxiosManager.post('/PriceList/UpdatePriceList', data),

            deletePriceList: (id, userId) =>
                AxiosManager.post('/PriceList/DeletePriceList', { id, deletedById: userId }),

            getCustomerList: () =>
                AxiosManager.get('/Customer/GetCustomerList'),

            getTaxList: () =>
                AxiosManager.get('/Tax/GetTaxList')

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

                const [customers, taxes] = await Promise.all([
                    services.getCustomerList(),
                    services.getTaxList()
                ]);
                state.customerListLookupData = customers?.data?.content?.data ?? [];
                state.taxListLookupData = taxes?.data?.content?.data ?? [];

            },

            loadPriceList: async (productId) => {
                const response = await services.getPriceListByProduct(productId);
                state.priceListData = response?.data?.content?.data ?? [];
                return state.priceListData;
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
                                ? 'Törölve'
                                : 'Mentve',

                            timer: 1500,
                            showConfirmButton: false

                        });


                        mainModal.obj.hide();

                    }

                }
                catch (e) {

                    Swal.fire({

                        icon: 'error',
                        title: 'Hiba történt',
                        text: e?.response?.data?.message ?? e.message,
                        confirmButtonText: 'OK'

                    });

                }
                finally {

                    state.isSubmitting = false;

                }

            },

            handleOpenPriceList: async () => {
                await window.__openProductPriceList(state.id, state.name);
            },

            handlePriceListSubmit: async () => {
                try {
                    state.plIsSubmitting = true;
                    const userId = StorageManager.getUserId();

                    if (state.plDeleteMode) {
                        await services.deletePriceList(state.plId, userId);
                    } else {
                        await services.updatePriceList({
                            id: state.plId,
                            productId: state.plProductId,
                            customerId: state.plCustomerId,
                            taxId: state.plTaxId,
                            netPrice: parseFloat(state.plNetPrice) || 0,
                            grossPrice: state.plGrossPrice ? parseFloat(state.plGrossPrice) : null,
                            quantityDiscount: state.plQuantityDiscount ? parseFloat(state.plQuantityDiscount) : null,
                            discountFrom: state.plDiscountFrom || null,
                            discountTo: state.plDiscountTo || null,
                            updatedById: userId
                        });
                    }

                    const data = await methods.loadPriceList(state.selectedProductId);
                    priceListGrid.obj.setProperties({ dataSource: data });

                    await methods.populateMainData();
                    mainGrid.refresh();

                    priceListEditModal.obj.hide();
                } catch (e) {
                    Swal.fire({ icon: 'error', title: 'Hiba', text: e?.response?.data?.message ?? e.message });
                } finally {
                    state.plIsSubmitting = false;
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
                                headerText: 'Kód',
                                width: 150
                            },

                            {
                                field: 'name',
                                headerText: 'Név',
                                width: 200
                            },

                            {
                                field: 'productGroupName',
                                headerText: 'Termékcsoport',
                                width: 150
                            },

                            {
                                field: 'unitPrice',
                                headerText: 'Egységár',
                                format: 'N2',
                                width: 120
                            },

                            {
                                headerText: '',
                                width: 100,
                                textAlign: 'Center',
                                template: '${if(hasPriceList)}<span class="badge bg-blue-lt">Egyedi árak</span>${/if}'
                            }

                        ],



                        toolbar: [

                            'ExcelExport',

                            'Search',

                            { type: 'Separator' },

                            {

                                text: 'Hozzáadás',
                                tooltipText: 'Hozzáadás',
                                id: 'AddCustom',
                                prefixIcon: 'e-add'

                            },

                            {

                                text: 'Szerkesztés',
                                tooltipText: 'Szerkesztés',
                                id: 'EditCustom',
                                prefixIcon: 'e-edit'

                            },

                            {

                                text: 'Törlés',
                                tooltipText: 'Törlés',
                                id: 'DeleteCustom',
                                prefixIcon: 'e-delete'

                            },

                            { type: 'Separator' },

                            {

                                text: 'Excel import',
                                tooltipText: 'Excel import',
                                id: 'ImportExcel',
                                prefixIcon: 'e-upload'

                            },

                            {

                                text: 'Sablon letöltés',
                                tooltipText: 'Sablon letöltés',
                                id: 'DownloadTemplate',
                                prefixIcon: 'e-download'

                            },


                        ],



                        dataBound: () => {

                            mainGrid.obj
                                .toolbarModule
                                .enableItems(

                                    ['EditCustom', 'DeleteCustom'],
                                    false

                                );

                            mainGrid.obj.autoFitColumns(['number', 'name', 'productGroupName', 'unitPrice']);

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

                                state.mainTitle = 'Termék hozzáadása';

                                mainModal.obj.show();

                            }



                            if (args.item.id === 'EditCustom') {

                                const r =
                                    mainGrid.obj.getSelectedRecords()[0];

                                if (!r)
                                    return;

                                state.deleteMode = false;

                                state.mainTitle = 'Termék módosítása';

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

                                state.mainTitle = 'Termék törlése?';

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

        const priceListModal = {
            obj: null,
            create: () => {
                priceListModal.obj = new bootstrap.Modal(priceListModalRef.value, { backdrop: 'static', keyboard: false });
            }
        };

        const priceListEditModal = {
            obj: null,
            create: () => {
                priceListEditModal.obj = new bootstrap.Modal(priceListEditModalRef.value, { backdrop: 'static', keyboard: false });
            }
        };

        let plCustomerDropdown = null;
        let plTaxDropdown = null;

        const priceListGrid = {
            obj: null,
            create: () => {
                priceListGrid.obj = new ej.grids.Grid({
                    height: '350px',
                    dataSource: [],
                    allowSorting: true,
                    allowFiltering: true,
                    allowPaging: true,
                    toolbar: [
                        {
                            text: 'Szerkesztés',
                            id: 'PlEdit',
                            prefixIcon: 'e-edit'
                        },
                        {
                            text: 'Törlés',
                            id: 'PlDelete',
                            prefixIcon: 'e-delete'
                        }
                    ],
                    columns: [
                        { field: 'customerName', headerText: 'Vevő', width: 180 },
                        { field: 'taxName', headerText: 'ÁFA', width: 100 },
                        { field: 'taxPercentage', headerText: 'ÁFA%', width: 80, format: 'N2' },
                        { field: 'netPrice', headerText: 'Nettó ár', width: 120, format: 'N2' },
                        { field: 'grossPrice', headerText: 'Bruttó ár', width: 120, format: 'N2' },
                        { field: 'quantityDiscount', headerText: 'Kedvezmény%', width: 120, format: 'N2' },
                        { field: 'discountFrom', headerText: 'Tól', width: 110, format: 'yyyy-MM-dd', type: 'date' },
                        { field: 'discountTo', headerText: 'Ig', width: 110, format: 'yyyy-MM-dd', type: 'date' }
                    ],
                    dataBound: () => {
                        priceListGrid.obj.toolbarModule.enableItems(['PlEdit', 'PlDelete'], false);
                    },
                    rowSelected: () => {
                        priceListGrid.obj.toolbarModule.enableItems(['PlEdit', 'PlDelete'], true);
                    },
                    rowDeselected: () => {
                        priceListGrid.obj.toolbarModule.enableItems(['PlEdit', 'PlDelete'], false);
                    },
                    toolbarClick: args => {
                        const r = priceListGrid.obj.getSelectedRecords()[0];
                        if (!r) return;

                        state.plId = r.id;
                        state.plProductId = r.productId;
                        state.plCustomerId = r.customerId;
                        state.plTaxId = r.taxId;
                        state.plNetPrice = r.netPrice;
                        state.plGrossPrice = r.grossPrice;
                        state.plQuantityDiscount = r.quantityDiscount;
                        state.plDiscountFrom = r.discountFrom ? new Date(r.discountFrom).toISOString().slice(0, 10) : null;
                        state.plDiscountTo = r.discountTo ? new Date(r.discountTo).toISOString().slice(0, 10) : null;

                        if (args.item.id === 'PlEdit') {
                            state.plDeleteMode = false;
                            state.plEditTitle = 'Árlista szerkesztése';
                        } else if (args.item.id === 'PlDelete') {
                            state.plDeleteMode = true;
                            state.plEditTitle = 'Árlista törlése';
                        }

                        if (plCustomerDropdown) plCustomerDropdown.value = r.customerId ?? null;
                        if (plTaxDropdown) plTaxDropdown.value = r.taxId ?? null;

                        priceListEditModal.obj.show();
                    }
                });
                priceListGrid.obj.appendTo(priceListGridRef.value);
            }
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


            window.__openProductPriceList = async (productId, productName) => {
                state.selectedProductId = productId;
                state.selectedProductName = productName;
                const data = await methods.loadPriceList(productId);
                priceListGrid.obj.setProperties({ dataSource: data });
                priceListModal.obj.show();
            };

            mainModal.create();
            importResultModal.create();
            priceListModal.create();
            priceListEditModal.create();
            priceListGrid.create();

            plCustomerDropdown = createDropdown(
                plCustomerIdRef,
                state.customerListLookupData,
                'id',
                'name',
                v => state.plCustomerId = v
            );

            plTaxDropdown = createDropdown(
                plTaxIdRef,
                state.taxListLookupData,
                'id',
                'name',
                v => state.plTaxId = v
            );

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
            priceListModalRef,
            priceListEditModalRef,
            priceListGridRef,
            plCustomerIdRef,
            plTaxIdRef,

            productGroupIdRef,
            unitMeasureIdRef,

            nameRef,
            numberRef,
            unitPriceRef,

            state,
            handler

        };

    }

};

Vue.createApp(App).mount('#app');