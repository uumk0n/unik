package com.company.pr9.screen.route;

import io.jmix.ui.screen.*;
import com.company.pr9.entity.Route;

@UiController("tr_Route.edit")
@UiDescriptor("route-edit.xml")
@EditedEntityContainer("routeDc")
public class RouteEdit extends StandardEditor<Route> {
}