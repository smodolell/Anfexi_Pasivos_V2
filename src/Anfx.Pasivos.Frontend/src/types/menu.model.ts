export interface MenuChild {
  label:  string;
  route:  string;
  roles:  string[];
  icon?:  string;
}

export interface MenuItem {
  id:           number;
  label:        string;
  icon:         string;
  routePrefix:  string;
  roles:        string[];
  children:     MenuChild[];
}
