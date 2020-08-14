export type NavigationState = {
  path: string,
  isDrawerOpen: boolean
};

export const defaultNavigationState: NavigationState = {
  path: '/',
  isDrawerOpen: false,
};
