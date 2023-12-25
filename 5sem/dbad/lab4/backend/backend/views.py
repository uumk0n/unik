from django.shortcuts import get_object_or_404
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_POST
from django.core.serializers import serialize
from .models import EduDocs, HFInfo, Institut, PersonalData, Work
from django.db.models import F
import json
from django.apps import apps

#EduDoc
@csrf_exempt
def edu_docs_list(request):
    if request.method == 'GET':
        edu_docs = EduDocs.objects.all()
        return JsonResponse(json.loads(serialize('json', edu_docs)), safe=False)

@csrf_exempt
def edu_doc_detail(request, inila):
    edu_doc = get_object_or_404(EduDocs, Inila=inila)

    if request.method == 'GET':
        return JsonResponse(json.loads(serialize('json', [edu_doc]))[0])

    elif request.method == 'PUT':
        data = json.loads(request.body.decode('utf-8'))
        edu_doc.SndEduDoc = data['SndEduDoc']
        edu_doc.DateEnd = data['DateEnd']
        edu_doc.RegEduDoc = data['RegEduDoc']
        edu_doc.save()
        return JsonResponse(json.loads(serialize('json', [edu_doc]))[0])

    elif request.method == 'DELETE':
        edu_doc.delete()
        return JsonResponse({'message': 'Deleted successfully'}, status=204)

    return JsonResponse({'error': 'Invalid request method'}, status=400)

@require_POST
@csrf_exempt
def create_edu_doc(request):
    data = json.loads(request.body.decode('utf-8'))
    edu_doc = EduDocs.objects.create(
        Inila=data['Inila'],
        SndEduDoc=data['SndEduDoc'],
        DateEnd=data['DateEnd'],
        RegEduDoc=data['RegEduDoc']
    )
    return JsonResponse(json.loads(serialize('json', [edu_doc]))[0], status=201)

#HfInfo
@csrf_exempt
def hf_infos_list(request):
    if request.method == 'GET':
        hf_infos = HFInfo.objects.all()
        return JsonResponse(json.loads(serialize('json', hf_infos)), safe=False)

@csrf_exempt
def hf_info_detail(request, inila):
    hf_info = get_object_or_404(HFInfo, Inila=inila)

    if request.method == 'GET':
        return JsonResponse(json.loads(serialize('json', [hf_info]))[0])

    elif request.method == 'PUT':
        data = json.loads(request.body.decode('utf-8'))
        hf_info.Status = data['Status']
        hf_info.IdInfo = data['IdInfo']
        hf_info.DateOrd = data['DateOrd']
        hf_info.RegOrgNum = data['RegOrgNum']
        hf_info.save()
        return JsonResponse(json.loads(serialize('json', [hf_info]))[0])

    elif request.method == 'DELETE':
        hf_info.delete()
        return JsonResponse({'message': 'Deleted successfully'}, status=204)

    return JsonResponse({'error': 'Invalid request method'}, status=400)

@require_POST
@csrf_exempt
def create_hf_info(request):
    data = json.loads(request.body.decode('utf-8'))
    hf_info = HFInfo.objects.create(
        Inila=data['Inila'],
        Status=data['Status'],
        IdInfo=data['IdInfo'],
        DateOrd=data['DateOrd'],
        RegOrgNum=data['RegOrgNum']
    )
    return JsonResponse(json.loads(serialize('json', [hf_info]))[0], status=201)

#PersonalData
@csrf_exempt
def personal_datas_list(request):
    if request.method == 'GET':
        personal_datas = PersonalData.objects.all()
        return JsonResponse(json.loads(serialize('json', personal_datas)), safe=False)

@csrf_exempt
def personal_data_detail(request, fcs):
    personal_data = get_object_or_404(PersonalData, Fcs=fcs)

    if request.method == 'GET':
        return JsonResponse(json.loads(serialize('json', [personal_data]))[0])

    elif request.method == 'PUT':
        data = json.loads(request.body.decode('utf-8'))
        personal_data.Name = data['Name']
        personal_data.Age = data['Age']
        # Add other fields as needed
        personal_data.save()
        return JsonResponse(json.loads(serialize('json', [personal_data]))[0])

    elif request.method == 'DELETE':
        personal_data.delete()
        return JsonResponse({'message': 'Deleted successfully'}, status=204)

    return JsonResponse({'error': 'Invalid request method'}, status=400)

@require_POST
@csrf_exempt
def create_personal_data(request):
    data = json.loads(request.body.decode('utf-8'))
    personal_data = PersonalData.objects.create(
        Fcs=data['Fcs'],
        Name=data['Name'],
        Age=data['Age'],
        # Add other fields as needed
    )
    return JsonResponse(json.loads(serialize('json', [personal_data]))[0], status=201)

#Work
@csrf_exempt
def works_list(request):
    if request.method == 'GET':
        works = Work.objects.all()
        return JsonResponse(json.loads(serialize('json', works)), safe=False)

@csrf_exempt
def work_detail(request, regorgnum):
    work = get_object_or_404(Work, RegOrgNum=regorgnum)

    if request.method == 'GET':
        return JsonResponse(json.loads(serialize('json', [work]))[0])

    elif request.method == 'PUT':
        data = json.loads(request.body.decode('utf-8'))
        work.Title = data['Title']
        work.Location = data['Location']
        # Add other fields as needed
        work.save()
        return JsonResponse(json.loads(serialize('json', [work]))[0])

    elif request.method == 'DELETE':
        work.delete()
        return JsonResponse({'message': 'Deleted successfully'}, status=204)

    return JsonResponse({'error': 'Invalid request method'}, status=400)

@require_POST
@csrf_exempt
def create_work(request):
    data = json.loads(request.body.decode('utf-8'))
    work = Work.objects.create(
        RegOrgNum=data['RegOrgNum'],
        Title=data['Title'],
        Location=data['Location'],
        # Add other fields as needed
    )
    return JsonResponse(json.loads(serialize('json', [work]))[0], status=201)

#Institut
@csrf_exempt
def instituts_list(request):
    if request.method == 'GET':
        instituts = Institut.objects.all()
        return JsonResponse(json.loads(serialize('json', instituts)), safe=False)

@csrf_exempt
def institut_detail(request, regedudoc):
    institut = get_object_or_404(Institut, RegEduDoc=regedudoc)

    if request.method == 'GET':
        return JsonResponse(json.loads(serialize('json', [institut]))[0])

    elif request.method == 'PUT':
        data = json.loads(request.body.decode('utf-8'))
        institut.Name = data['Name']
        institut.Location = data['Location']
        # Add other fields as needed
        institut.save()
        return JsonResponse(json.loads(serialize('json', [institut]))[0])

    elif request.method == 'DELETE':
        institut.delete()
        return JsonResponse({'message': 'Deleted successfully'}, status=204)

    return JsonResponse({'error': 'Invalid request method'}, status=400)

@require_POST
@csrf_exempt
def create_institut(request):
    data = json.loads(request.body.decode('utf-8'))
    institut = Institut.objects.create(
        RegEduDoc=data['RegEduDoc'],
        Name=data['Name'],
        Location=data['Location'],
        # Add other fields as needed
    )
    return JsonResponse(json.loads(serialize('json', [institut]))[0], status=201)

@csrf_exempt
def get_combo_box_options(request, model_name, column_name):
    if request.method == 'GET':
        # Define a dictionary to map model names to model classes
        model_mapping = {
            'PersonalData': apps.get_model('backend', 'PersonalData'),
            'HFInfo': apps.get_model('backend', 'HFInfo'),
            'EduDoc': apps.get_model('backend', 'EduDocs'),
            'Institut': apps.get_model('backend', 'Institut'),
            'Work': apps.get_model('backend', 'Work'),
        }

        model = model_mapping.get(model_name)

        if model:
            options = model.objects.values_list(column_name, flat=True).distinct()
            print(options)
            return JsonResponse(list(options), safe=False)
        else:
            return JsonResponse({'error': f'Model {model_name} not found'}, status=400)
    
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
import json
from .models import EduDocs, HFInfo, Institut, PersonalData, Work

@csrf_exempt
def generate_report(request):
    if request.method == 'POST':
        try:
            data = json.loads(request.body.decode('utf-8'))
            table_names = data

            combined_data_table = {}

            for table_name in table_names:
                table_data = []
                if table_name == 'EduDocs':
                    table_data = EduDocs.objects.values()
                elif table_name == 'HfInfos':
                    table_data = HFInfo.objects.values()
                elif table_name == 'Institutes':
                    table_data = Institut.objects.values()
                elif table_name == 'PersonalData':
                    table_data = PersonalData.objects.values()
                elif table_name == 'Works':
                    table_data = Work.objects.values()
                # Add more cases for other tables as needed

                # Convert the queryset to a list of dictionaries
                table_data_list = list(table_data)

                # Add the table data to the combined_data_table
                combined_data_table[table_name] = table_data_list

            return JsonResponse(combined_data_table, safe=False)

        except json.JSONDecodeError as e:
            return JsonResponse({'error': 'Invalid JSON format'}, status=400)

        except Exception as e:
            return JsonResponse({'error': f'Error: {str(e)}'}, status=500)

    return JsonResponse({'error': 'Invalid request method'}, status=400)

