export interface MenuChild {
  label:  string;
  route:  string;
  roles?: string[];
  icon?:  string;
  order?: number;
}

export interface MenuItem {
  id:        string;
  label:     string;
  icon:      string;
  route?:    string;
  roles?:    string[];
  order?:    number;
  children:  MenuChild[];
}
