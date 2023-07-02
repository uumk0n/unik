package com.company.pr9.screen.bus;

import io.jmix.ui.screen.*;
import com.company.pr9.entity.Bus;

@UiController("tr_Bus.edit")
@UiDescriptor("bus-edit.xml")
@EditedEntityContainer("busDc")
public class BusEdit extends StandardEditor<Bus> {
}