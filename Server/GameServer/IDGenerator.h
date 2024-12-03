#pragma once
#include <bitset>

class IDGenerator
{
public:
    // �ִ� ID �� (uint16 �ִ밪 65535)
    static constexpr uint16_t MAX_ID = 65535;

    // Generate a unique ID
    uint16_t GenerateID() 
    {
        std::lock_guard<std::mutex> lock(mutex);

        for (uint16_t i = 0; i <= MAX_ID; ++i) {
            // ��ȯ�ϸ� ID�� ���� (currentID�� �������� ��ȯ)
            uint16_t id = (currentID + i) % (MAX_ID + 1);

            // ID�� ��� ������ ���� ��� �Ҵ�
            if (!usedIDs.test(id)) {
                usedIDs.set(id);  // �ش� ID�� ��� ������ ǥ��
                currentID = id + 1;  // ���� ���� ��ġ�� ����
                return id;
            }
        }

        // ��� ID�� ��� ���� ��� ���ܸ� �߻���ų �� ����
        throw std::runtime_error("No available IDs");
    }

    // Release an ID when it is no longer in use
    void ReleaseID(uint16_t id) 
    {
        std::lock_guard<std::mutex> lock(mutex);
        usedIDs.reset(id);  // ID�� �̻�� ���·� ǥ��
    }

private:
    std::bitset<MAX_ID + 1> usedIDs; // ID ��� ���¸� �����ϴ� ��Ʈ��
    uint16_t currentID = 0;          // �������� ������ ID�� ������
    std::mutex mutex;                // ��Ƽ������ ȯ�濡���� ����ȭ�� ���� ���ؽ�
};

