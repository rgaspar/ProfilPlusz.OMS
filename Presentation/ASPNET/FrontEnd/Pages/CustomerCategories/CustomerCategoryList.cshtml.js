const App = {
    setup() {
        const state = Vue.reactive({
            mainData: [],
            deleteMode: false,
            mainTitle: null,
            id: '',
            name: '',
            description: '',
            errors: { name: '' },
            isSubmitting: false
        });

        const mainGridRef = Vue.ref(null);
        const mainModalRef = Vue.ref(null);

        const services = {
            getMainData: () => AxiosManager.get('/CustomerCategory/GetCustomerCategoryList', {}),
            createMainData: (name, description, createdById) =>
                AxiosManager.post('/CustomerCategory/CreateCustomerCategory', { name, description, createdById }),
            updateMainData: (id, name, description, updatedById) =>
                AxiosManager.post('/CustomerCategory/UpdateCustomerCategory', { id, name, description, updatedById }),
            deleteMainData: (id, deletedById) =>
                AxiosManager.post('/CustomerCategory/DeleteCustomerCategory', { id, deletedById }),
        };

        const methods = {
            populateMainData: async () => {
                const response = await services.getMainData();
                state.mainData = (response?.data?.content?.data ?? []).map(item => ({
                    ...item,
                    createdAtUtc: new Date(item.createdAtUtc)
                }));
            }
        };

        const resetFormState = () => {
            state.id = '';
            state.name = '';
            state.description = '';
            state.errors = { name: '' };
        };

        const handler = {
            handleSubmit: async function () {
                try {
                    state.isSubmitting = true;
                    await new Promise(resolve => setTimeout(resolve, 200));

                    if (!state.name) {
                        state.errors.name = 'Név megadása kötelező.';
                        return;
                    }

                    const response = state.id === ''
                        ? await services.createMainData(state.name, state.description, StorageManager.getUserId())
                        : state.deleteMode
                            ? await services.deleteMainData(state.id, StorageManager.getUserId())
                            : await services.updateMainData(state.id, state.name, state.description, StorageManager.getUserId());

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
                        { field: 'name', headerText: 'Név', width: 200, minWidth: 200 },
                        { field: 'description', headerText: 'Leírás', width: 400, minWidth: 400 },
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
                        mainGrid.obj.autoFitColumns(['name', 'description', 'createdAtUtc']);
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
                            state.mainTitle = 'Ügyfélkategória hozzáadása';
                            resetFormState();
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'EditCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = false;
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            state.mainTitle = 'Ügyfélkategória szerkesztése';
                            state.id = r.id ?? '';
                            state.name = r.name ?? '';
                            state.description = r.description ?? '';
                            mainModal.obj.show();
                        }

                        if (args.item.id === 'DeleteCustom' && mainGrid.obj.getSelectedRecords().length) {
                            state.deleteMode = true;
                            const r = mainGrid.obj.getSelectedRecords()[0];
                            state.mainTitle = 'Ügyfélkategória törlése?';
                            state.id = r.id ?? '';
                            state.name = r.name ?? '';
                            state.description = r.description ?? '';
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
                await SecurityManager.authorizePage(['CustomerCategories']);
                await SecurityManager.validateToken();
                await methods.populateMainData();
                await mainGrid.create(state.mainData);
                mainModal.create();
            } catch (e) {
                console.error('page init error:', e);
            }
        });

        return { mainGridRef, mainModalRef, state, handler };
    }
};

Vue.createApp(App).mount('#app');
