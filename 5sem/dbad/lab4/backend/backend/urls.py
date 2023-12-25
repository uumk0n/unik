"""
URL configuration for backend project.

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/5.0/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.urls import path
from .views import edu_docs_list, edu_doc_detail, create_edu_doc, \
    hf_infos_list, hf_info_detail, create_hf_info, \
    instituts_list, institut_detail, create_institut, \
    personal_datas_list, personal_data_detail, create_personal_data, \
    works_list, work_detail, create_work, get_combo_box_options, generate_report

urlpatterns = [
    path('edu-docs/', edu_docs_list, name='edu_docs_list'),
    path('edu-docs/<str:inila>/', edu_doc_detail, name='edu_doc_detail'),
    path('edu-docs/create/', create_edu_doc, name='create_edu_doc'),
    
    path('hf-infos/', hf_infos_list, name='hf_infos_list'),
    path('hf-infos/<str:inila>/', hf_info_detail, name='hf_info_detail'),
    path('hf-infos/create/', create_hf_info, name='create_hf_info'),

    path('instituts/', instituts_list, name='instituts_list'),
    path('instituts/<str:regedudoc>/', institut_detail, name='institut_detail'),
    path('instituts/create/', create_institut, name='create_institut'),

    path('personal-datas/', personal_datas_list, name='personal_datas_list'),
    path('personal-datas/<str:fcs>/', personal_data_detail, name='personal_data_detail'),
    path('personal-datas/create/', create_personal_data, name='create_personal_data'),

    path('works/', works_list, name='works_list'),
    path('works/<str:regorgnum>/', work_detail, name='work_detail'),
    path('works/create/', create_work, name='create_work'),

    path('get_combo_box_options/<str:model_name>/<str:column_name>/', get_combo_box_options, name='get_combo_box_options'),

    path('generate_report/', generate_report, name='generate_report'),
]
