// Global Keyboard Shortcuts Handler for ERP Application
// Handles Alt+S for Save and Alt+F for Search Focus

window.keyboardShortcuts = {
    dotNetHelper: null,
    keydownHandler: null,
    isSaving: false, // Debounce flag to prevent multiple saves

    // Initialize keyboard shortcuts
    initialize: function (dotNetHelper) {
        console.log('Initializing keyboard shortcuts...', dotNetHelper);
        this.dotNetHelper = dotNetHelper;

        // Remove old handler if exists to prevent duplicates
        this.dispose();

        // Attach new handler
        this.attachGlobalHandler();
        console.log('Keyboard shortcuts initialized successfully');
    },

    // Attach global keyboard event handler
    attachGlobalHandler: function () {
        const self = this; // Store reference to keyboardShortcuts object

        // Create the handler function
        this.keydownHandler = async (e) => {
            // Alt+S - Save functionality
            if (e.altKey && (e.key === 's' || e.key === 'S')) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                // Prevent multiple rapid saves with debounce
                if (self.isSaving) {
                    console.log('Save already in progress, ignoring...');
                    return false;
                }

                console.log('Alt+S pressed - Triggering Save');
                self.isSaving = true; // Set flag to prevent duplicate saves

                // Only notify .NET component (don't click button to avoid double save)
                if (self.dotNetHelper) {
                    try {
                        await self.dotNetHelper.invokeMethodAsync('HandleSaveShortcut');
                        console.log('.NET HandleSaveShortcut called');
                    } catch (error) {
                        console.log('Error calling HandleSaveShortcut:', error);
                    } finally {
                        // Reset flag after 500ms to allow next save
                        setTimeout(() => {
                            self.isSaving = false;
                            console.log('Save debounce reset');
                        }, 500);
                    }
                } else {
                    // Fallback: If no .NET helper, click the button
                    const saveButton = document.querySelector('button[type="submit"], .e-primary[type="submit"], button.save-btn');
                    if (saveButton && !saveButton.disabled) {
                        saveButton.click();
                        console.log('Save button clicked (fallback)');
                    }
                    // Reset flag after 500ms
                    setTimeout(() => {
                        self.isSaving = false;
                    }, 500);
                }

                return false;
            }


            if (e.altKey && (e.key === 'f' || e.key === 'F')) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();

                const active = document.activeElement;
                if (active?.dataset?.ignoreAltF === "true") {
                    active.blur();
                }

                const focusGridSearch = () => {
                    let gridSearchInput =
                        document.querySelector('.e-grid .e-toolbar-right input.e-input') ||
                        document.querySelector('.e-grid .e-toolbar input.e-control.e-input') ||
                        document.querySelector('.e-grid .e-toolbar input[type="text"]') ||
                        document.querySelector('.e-grid .e-toolbar .e-input-group input');

                    if (gridSearchInput) {
                        gridSearchInput.focus({ preventScroll: true });
                        gridSearchInput.select();
                        console.log('Grid search focused successfully');
                        return true;
                    }
                    return false;
                };

                if (!focusGridSearch()) {
                    let attempts = 0;
                    const interval = setInterval(() => {
                        attempts++;
                        if (focusGridSearch() || attempts > 30) {
                            clearInterval(interval);
                            if (attempts > 30) {
                                console.warn('Grid search input still not found after retries');
                            }
                        }
                    }, 100);
                    console.warn('Grid search not found immediately, retrying...');
                }

                if (self.dotNetHelper) {
                    await self.dotNetHelper.invokeMethodAsync('HandleSearchShortcut');
                }

                return false;
            }

            // Alt+A - Add New Record
            if (e.altKey && (e.key === 'a' || e.key === 'A')) {
                e.preventDefault();
                console.log('Alt+A pressed - Triggering Add New');

                if (self.dotNetHelper) {
                    try {
                        await self.dotNetHelper.invokeMethodAsync('HandleAddShortcut');
                        console.log('.NET HandleAddShortcut called');
                    } catch (error) {
                        console.log('Error calling HandleAddShortcut:', error);
                    }
                }

                const addButton = document.querySelector('.add-new-btn');
                if (addButton && !addButton.disabled) {
                    addButton.click();
                    console.log('Add button clicked via .add-new-btn class');
                }

                return false;
            }
        };

        // Add the event listener
        document.addEventListener('keydown', this.keydownHandler);
        console.log('Keyboard shortcut listener attached');
    },

    focusElement: function (selector) {
        const el = document.querySelector(selector);
        if (el) {
            el.focus();
            el.select?.();
            return true;
        }
        return false;
    },

    clickElement: function (selector) {
        const el = document.querySelector(selector);
        if (el && !el.disabled) {
            el.click();
            return true;
        }
        return false;
    },

    dispose: function () {
        // Remove the event listener if it exists
        if (this.keydownHandler) {
            document.removeEventListener('keydown', this.keydownHandler);
            this.keydownHandler = null;
            console.log('Keyboard event listener removed');
        }

        this.dotNetHelper = null;
        this.isSaving = false; // Reset saving flag
    }
};